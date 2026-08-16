using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using QuranSchool.Api.DTOs.DatabaseBackup;
using QuranSchool.Api.Exceptions;
using QuranSchool.Api.Models;
using QuranSchool.Api.Repositories.Interfaces;
using QuranSchool.Api.Services.Interfaces;

namespace QuranSchool.Api.Services.Implementations;

/// <summary>
/// منطق النسخ الاحتياطي والاستعادة عبر أدوات PostgreSQL الأصلية
/// (pg_dump / pg_restore) مع تخزين الملفات عبر [IBackupStorage] المستقل
/// عن بيئة الاستضافة. يضمن عدم تنفيذ أكثر من عملية نسخ/استعادة في نفس
/// الوقت، ويطبّق سياسة الاحتفاظ بالنسخ، ويسجّل كل إجراء في سجل التدقيق.
/// </summary>
public class BackupService(
    IDatabaseBackupRepository repository,
    IBackupStorage storage,
    IConfiguration configuration,
    ILogger<BackupService> logger) : IBackupService
{
    /// <summary>قفل يمنع تشغيل أكثر من عملية نسخ/استعادة واحدة في نفس اللحظة.</summary>
    private static readonly SemaphoreSlim OperationLock = new(1, 1);

    public async Task<List<DatabaseBackupDto>> GetBackupsAsync()
        => (await repository.GetAllAsync()).Select(MapToDto).ToList();

    public async Task<BackupSummaryDto> GetSummaryAsync()
    {
        var backups = await repository.GetAllAsync();
        var settings = await repository.GetSettingAsync();
        var last = backups.FirstOrDefault(b => b.Status == "Success");

        return new BackupSummaryDto
        {
            LastBackupDate = last?.CreatedDate,
            LastBackupFileName = last?.FileName,
            NextScheduledBackup = settings is { IsEnabled: true } ? settings.NextRunAt : null,
            AutomaticBackupEnabled = settings?.IsEnabled ?? false,
            TotalBackups = backups.Count,
            TotalSize = backups.Sum(b => b.FileSize),
            BackupDirectory = await ResolveTargetDirectoryAsync(null)
        };
    }

    public async Task<DatabaseBackupDto> CreateBackupAsync(int adminId, string adminName, string? directory = null)
    {
        await OperationLock.WaitAsync();
        try
        {
            await RunBackupAsync("Manual", adminId, adminName, directory);
            var latest = await repository.GetLatestAsync();
            return MapToDto(latest ?? throw new BadRequestException("تعذر تسجيل النسخة الاحتياطية."));
        }
        catch (BadRequestException)
        {
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Manual backup failed unexpectedly");
            throw new BadRequestException("فشل إنشاء النسخة الاحتياطية. حاول مرة أخرى.");
        }
        finally
        {
            OperationLock.Release();
        }
    }

    public async Task<(byte[] Data, string FileName)> DownloadAsync(int backupId, int adminId, string adminName)
    {
        var backup = await GetExistingAsync(backupId);
        if (backup.Status != "Success")
        {
            throw new BadRequestException("لا يمكن تنزيل نسخة احتياطية فاشلة.");
        }

        byte[] data;
        try
        {
            data = await storage.ReadAsync(backup.FileName, backup.Directory);
        }
        catch (FileNotFoundException)
        {
            throw new NotFoundException("ملف النسخة الاحتياطية غير موجود على الخادم.");
        }

        await LogAuditAsync("Download", backup, adminId, adminName, null);
        return (data, backup.FileName);
    }

    public async Task RestoreAsync(int backupId, int adminId, string adminName)
    {
        var backup = await GetExistingAsync(backupId);
        if (backup.Status != "Success")
        {
            throw new BadRequestException("لا يمكن استعادة نسخة احتياطية فاشلة.");
        }

        var filePath = storage.GetAbsolutePath(backup.FileName, backup.Directory);
        if (!File.Exists(filePath))
        {
            throw new NotFoundException("ملف النسخة الاحتياطية غير موجود على الخادم.");
        }

        await OperationLock.WaitAsync();
        try
        {
            await ValidateDumpFileAsync(filePath);
            await CreateSafetySnapshotAsync();

            try
            {
                await RunPgRestoreAsync(filePath);
                backup.RestoreDate = DateTime.UtcNow;
                backup.RestoredBy = adminId;
                backup.RestoreStatus = "Success";
                await repository.SaveChangesAsync();
                await LogAuditAsync("Restore", backup, adminId, adminName, null);
            }
            catch (Exception ex)
            {
                backup.RestoreDate = DateTime.UtcNow;
                backup.RestoredBy = adminId;
                backup.RestoreStatus = "Failed";
                await repository.SaveChangesAsync();
                await LogAuditAsync("Restore", backup, adminId, adminName, Shorten(ex.Message, 500));
                logger.LogError(ex, "Restore failed for backup {Id}", backupId);
                throw new BadRequestException("فشلت عملية الاستعادة: " + Shorten(ex.Message, 500));
            }
        }
        finally
        {
            OperationLock.Release();
        }
    }

    public async Task<DatabaseBackupDto> RestoreFromFileAsync(IFormFile file, int adminId, string adminName)
    {
        if (file is null || file.Length == 0)
        {
            throw new BadRequestException("يرجى اختيار ملف نسخة احتياطية صالح.");
        }

        string filePath = string.Empty;
        await OperationLock.WaitAsync();
        try
        {
            var targetDirectory = await ResolveTargetDirectoryAsync(null);
            var fileName = $"upload_restore_{DateTime.UtcNow:yyyy-MM-dd_HH-mm-ss}.backup";
            filePath = storage.GetAbsolutePath(fileName, targetDirectory);

            await using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                await file.CopyToAsync(stream);
            }

            await ValidateDumpFileAsync(filePath);
            await CreateSafetySnapshotAsync();
            await RunPgRestoreAsync(filePath);

            var size = new FileInfo(filePath).Length;
            var backup = new DatabaseBackup
            {
                FileName = fileName,
                FilePath = filePath,
                Directory = targetDirectory,
                FileSize = size,
                CreatedDate = DateTime.UtcNow,
                BackupType = "Manual",
                Status = "Success",
                CreatedBy = adminId,
                CreatedByName = adminName,
                RestoreDate = DateTime.UtcNow,
                RestoredBy = adminId,
                RestoreStatus = "Success"
            };
            await repository.AddAsync(backup);
            await repository.SaveChangesAsync();
            await LogAuditAsync("RestoreFromFile", backup, adminId, adminName, null);
            return MapToDto(backup);
        }
        catch (Exception ex)
        {
            if (filePath.Length > 0 && File.Exists(filePath))
            {
                try
                {
                    File.Delete(filePath);
                }
                catch
                {
                    // تجاهل فشل تنظيف الملف المرفوع.
                }
            }

            if (ex is BadRequestException or NotFoundException)
            {
                throw;
            }
            logger.LogError(ex, "Restore from uploaded file failed");
            throw new BadRequestException("فشلت عملية الاستعادة من الملف: " + Shorten(ex.Message, 500));
        }
        finally
        {
            OperationLock.Release();
        }
    }

    public async Task DeleteAsync(int backupId, int adminId, string adminName)
    {
        var backup = await GetExistingAsync(backupId);
        await storage.DeleteAsync(backup.FileName, backup.Directory);
        await repository.DeleteAsync(backup);
        await repository.SaveChangesAsync();
        await LogAuditAsync("Delete", backup, adminId, adminName, null);
    }

    public async Task<BackupSettingsDto> GetSettingsAsync()
    {
        var dto = MapSettings(await repository.GetSettingAsync());
        if (string.IsNullOrWhiteSpace(dto.BackupDirectory))
        {
            dto.BackupDirectory = await ResolveTargetDirectoryAsync(null);
        }
        return dto;
    }

    public async Task<BackupSettingsDto> UpdateSettingsAsync(BackupSettingsDto dto, int adminId, string adminName)
    {
        ValidateSettings(dto);

        var setting = await repository.GetSettingAsync();
        if (setting is null)
        {
            setting = new DatabaseBackupSetting();
            setting.Id = 0;
        }

        setting.IsEnabled = dto.IsEnabled;
        setting.Frequency = dto.Frequency;
        setting.BackupTime = dto.BackupTime;
        setting.MaxBackupsToKeep = dto.MaxBackupsToKeep;
        setting.BackupDirectory = string.IsNullOrWhiteSpace(dto.BackupDirectory) ? null : dto.BackupDirectory.Trim();
        setting.UpdatedAt = DateTime.UtcNow;
        setting.UpdatedBy = adminId;
        setting.NextRunAt = ComputeNextRun(DateTime.UtcNow, dto.Frequency, dto.BackupTime, setting.LastRunAt);

        await repository.SaveSettingAsync(setting);
        await repository.SaveChangesAsync();
        await LogAuditAsync("UpdateSettings", null, adminId, adminName,
            $"is_enabled={dto.IsEnabled}; frequency={dto.Frequency}; time={dto.BackupTime}; " +
            $"keep={dto.MaxBackupsToKeep}; directory={setting.BackupDirectory ?? "(default)"}");

        return MapSettings(await repository.GetSettingAsync());
    }

    public async Task<List<BackupAuditLogDto>> GetAuditLogsAsync(int limit = 50)
        => (await repository.GetAuditLogsAsync(limit))
            .Select(l => new BackupAuditLogDto
            {
                Id = l.Id,
                BackupId = l.BackupId,
                BackupFileName = l.BackupFileName,
                Action = l.Action,
                PerformedByName = l.PerformedByName,
                PerformedAt = l.PerformedAt,
                Details = l.Details
            })
            .ToList();

    public DirectoryBrowserDto GetDirectoryStructure(string? path)
    {
        string current;
        if (string.IsNullOrWhiteSpace(path))
        {
            var desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            current = !string.IsNullOrWhiteSpace(desktop) && Directory.Exists(desktop)
                ? desktop
                : Directory.GetDirectoryRoot(Environment.CurrentDirectory);
        }
        else
        {
            try
            {
                current = Path.GetFullPath(path);
                if (!Directory.Exists(current))
                {
                    current = Directory.GetDirectoryRoot(current);
                }
            }
            catch
            {
                current = Directory.GetDirectoryRoot(Environment.CurrentDirectory);
            }
        }

        var entries = new List<DirectoryEntryDto>();
        try
        {
            foreach (var dir in Directory.EnumerateDirectories(current))
            {
                var info = new DirectoryInfo(dir);
                entries.Add(new DirectoryEntryDto { Name = info.Name, Path = info.FullName });
            }
        }
        catch (Exception ex) when (ex is UnauthorizedAccessException or IOException)
        {
            // المجلدات غير القابلة للقراءة تُتجاهل ببساطة.
        }

        var parent = Directory.GetParent(current)?.FullName;
        return new DirectoryBrowserDto
        {
            CurrentPath = current,
            ParentPath = parent,
            IsRoot = parent is null,
            Entries = entries
                .OrderBy(e => e.Name, StringComparer.OrdinalIgnoreCase)
                .ToList()
        };
    }

    public async Task RunScheduledBackupIfDueAsync()
    {
        var setting = await repository.GetSettingAsync();
        if (setting is null || !setting.IsEnabled || setting.NextRunAt is null)
        {
            return;
        }

        if (DateTime.UtcNow < setting.NextRunAt)
        {
            return;
        }

        try
        {
            await OperationLock.WaitAsync();
            try
            {
                await RunBackupAsync("Automatic", null, "تلقائي", null);
                setting.LastRunAt = DateTime.UtcNow;
                setting.NextRunAt = ComputeNextRun(DateTime.UtcNow, setting.Frequency, setting.BackupTime, setting.LastRunAt);
            }
            finally
            {
                OperationLock.Release();
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Automatic backup failed; scheduling a retry in one hour");
            setting.NextRunAt = DateTime.UtcNow.AddHours(1);
        }

        try
        {
            await repository.SaveSettingAsync(setting);
            await repository.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to persist backup settings after automatic run");
        }
    }

    // ───────────────────────────── private ─────────────────────────────

    private async Task RunBackupAsync(string backupType, int? adminId, string adminName, string? directory)
    {
        string fileName = string.Empty;
        string filePath = string.Empty;
        string targetDirectory = string.Empty;

        try
        {
            (fileName, filePath, targetDirectory, var size) = await RunPgDumpAsync(directory);

            var backup = new DatabaseBackup
            {
                FileName = fileName,
                FilePath = filePath,
                Directory = targetDirectory,
                FileSize = size,
                CreatedDate = DateTime.UtcNow,
                BackupType = backupType,
                Status = "Success",
                CreatedBy = adminId,
                CreatedByName = adminName
            };

            await repository.AddAsync(backup);
            await repository.SaveChangesAsync();
            await LogAuditAsync("Create", backup, adminId, adminName, null);

            // تطبيق سياسة الاحتفاظ على النسخ التلقائية فقط: عند بلوغ الحد
            // الأقصى (مثل 10 نسخ) تُحذف النسخة الأقدم تلقائيًا.
            if (backupType == "Automatic")
            {
                await EnforceRetentionAsync(targetDirectory);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Backup ({Type}) failed", backupType);

            try
            {
                if (filePath.Length > 0 && File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
            catch
            {
                // تجاهل فشل تنظيف الملف الجزئي.
            }

            var failed = new DatabaseBackup
            {
                FileName = fileName.Length > 0
                    ? fileName
                    : $"quran_school_{DateTime.UtcNow:yyyy-MM-dd_HH-mm}.backup",
                FilePath = filePath,
                Directory = targetDirectory,
                FileSize = 0,
                CreatedDate = DateTime.UtcNow,
                BackupType = backupType,
                Status = "Failed",
                CreatedBy = adminId,
                CreatedByName = adminName
            };

            await repository.AddAsync(failed);
            await repository.SaveChangesAsync();
            await LogAuditAsync("Create", failed, adminId, adminName, Shorten(ex.Message, 500));

            var reason = ex is BadRequestException && !string.IsNullOrWhiteSpace(ex.Message)
                ? Shorten(ex.Message, 300)
                : "تحقق من اتصال قاعدة البيانات وتثبيت أدوات PostgreSQL في بيئة الاستضافة.";
            throw new BadRequestException($"فشل إنشاء النسخة الاحتياطية. {reason}");
        }
    }

    private async Task<(string FileName, string FilePath, string Directory, long Size)> RunPgDumpAsync(string? directory)
    {
        var now = DateTime.UtcNow;
        var fileName = $"quran_school_{now:yyyy-MM-dd_HH-mm}.backup";
        var targetDirectory = await ResolveTargetDirectoryAsync(directory);
        var filePath = storage.GetAbsolutePath(fileName, targetDirectory);

        await RunPgDumpProcessAsync(filePath);

        var size = new FileInfo(filePath).Length;
        return (fileName, filePath, targetDirectory, size);
    }

    private async Task RunPgDumpProcessAsync(string filePath)
    {
        var (host, port, db, user, password, sslMode) = GetDbInfo();
        var psi = new ProcessStartInfo("pg_dump")
        {
            UseShellExecute = false,
            RedirectStandardError = true,
            CreateNoWindow = true
        };
        psi.EnvironmentVariables["PGPASSWORD"] = password;
        if (sslMode.Length > 0)
        {
            psi.EnvironmentVariables["PGSSLMODE"] = sslMode;
        }
        foreach (var arg in new[] { "-h", host, "-p", port, "-U", user, "-d", db, "-Fc", "-f", filePath })
        {
            psi.ArgumentList.Add(arg);
        }

        var process = StartPgTool(psi, "pg_dump");
        var stderr = await process.StandardError.ReadToEndAsync();
        await process.WaitForExitAsync();

        if (process.ExitCode != 0)
        {
            throw new BadRequestException($"pg_dump exited with code {process.ExitCode}: {Shorten(stderr, 500)}");
        }
    }

    private static Process StartPgTool(ProcessStartInfo psi, string toolName)
    {
        try
        {
            return Process.Start(psi)
                ?? throw new BadRequestException($"تعذر تشغيل {toolName}. تأكد من تثبيت أدوات PostgreSQL.");
        }
        catch (System.ComponentModel.Win32Exception)
        {
            throw new BadRequestException(
                $"تعذر العثور على أداة {toolName} في بيئة الاستضافة. " +
                "تأكد من تثبيت أدوات PostgreSQL (postgresql-client) في الصورة.");
        }
    }

    /// <summary>
    /// التحقق من أن الملف نسخة احتياطية صالحة (pg_restore --list) قبل
    /// البدء بأي عملية استعادة — يمنع محاولة استعادة ملفات تالفة أو غير
    /// مدعومة تترك قاعدة البيانات في حالة جزئية.
    /// </summary>
    private static async Task ValidateDumpFileAsync(string filePath)
    {
        var psi = new ProcessStartInfo("pg_restore")
        {
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };
        psi.ArgumentList.Add("--list");
        psi.ArgumentList.Add(filePath);

        try
        {
            var process = StartPgTool(psi, "pg_restore");
            var output = await process.StandardOutput.ReadToEndAsync();
            var stderr = await process.StandardError.ReadToEndAsync();
            await process.WaitForExitAsync();

            if (process.ExitCode != 0 || !output.Contains("TABLE DATA", StringComparison.Ordinal))
            {
                throw new BadRequestException("ملف النسخة الاحتياطية غير صالح أو غير مدعوم.");
            }
        }
        catch (BadRequestException)
        {
            throw;
        }
        catch (Exception)
        {
            throw new BadRequestException("ملف النسخة الاحتياطية غير صالح أو غير مدعوم.");
        }
    }

    /// <summary>
    /// إنشاء نسخة أمان تلقائية من قاعدة البيانات الحالية قبل تنفيذ أي
    /// استعادة، تُحفظ في مجلد النسخ الاحتياطية كشبكة أمان إضافية (لا
    /// تُسجَّل كسجل نسخة لأنها قد تُستبدل أثناء الاستعادة نفسها).
    /// </summary>
    private async Task<string> CreateSafetySnapshotAsync()
    {
        var targetDirectory = await ResolveTargetDirectoryAsync(null);
        var fileName = $"pre_restore_{DateTime.UtcNow:yyyy-MM-dd_HH-mm-ss}.backup";
        var filePath = storage.GetAbsolutePath(fileName, targetDirectory);

        await RunPgDumpProcessAsync(filePath);
        logger.LogInformation("Pre-restore safety snapshot created at {Path}", filePath);
        return filePath;
    }

    /// <summary>تنفيذ pg_restore بأمان: يُلف جميع العمليات في معاملة واحدة،
    /// فإذا فشلت أي خطوة تُتراجع كل التغييرات ويبقى الوضع الراهن كما هو.</summary>
    private async Task RunPgRestoreAsync(string filePath)
    {
        var tocPath = await CreateFilteredTocFileAsync(filePath);
        try
        {
            var (host, port, db, user, password, sslMode) = GetDbInfo();
            var psi = new ProcessStartInfo("pg_restore")
            {
                UseShellExecute = false,
                RedirectStandardError = true,
                CreateNoWindow = true
            };
            psi.EnvironmentVariables["PGPASSWORD"] = password;
            if (sslMode.Length > 0)
            {
                psi.EnvironmentVariables["PGSSLMODE"] = sslMode;
            }
            foreach (var arg in new[]
            {
                "-h", host, "-p", port, "-U", user, "-d", db,
                "--clean", "--if-exists", "--no-owner", "--no-privileges",
                "--exit-on-error", "--single-transaction",
                "--no-comments", "--no-acl",
                "-Fc", "-L", tocPath, filePath
            })
            {
                psi.ArgumentList.Add(arg);
            }

            var process = StartPgTool(psi, "pg_restore");
            var stderr = await process.StandardError.ReadToEndAsync();
            await process.WaitForExitAsync();

            if (process.ExitCode != 0)
            {
                throw new BadRequestException("pg_restore exited with code " + process.ExitCode + ": " + Shorten(stderr, 500));
            }
        }
        finally
        {
            try
            {
                if (File.Exists(tocPath))
                {
                    File.Delete(tocPath);
                }
            }
            catch
            {
                // تنظيف أفضل جهد — فشل الحذف لا يُفشل الاستعادة.
            }
        }
    }

    /// <summary>
    /// إنشاء قائمة استعادة (TOC) من ملف النسخة الاحتياطية مع استبعاد الكائنات
    /// التي لا يملكها مستخدم التطبيق، مثل ملحق pg_stat_statements المثبّت
    /// مسبقًا بواسطة Render ومملوك لمستخدم النظام postgres. لا يمكن لمستخدم
    /// التطبيق إسقاط هذا الملحق، لذا يجب استبعاده من قائمة الاستعادة وإلا
    /// فشلت العملية كلها (ويظل الملحق موجودًا في قاعدة البيانات كما هو).
    /// </summary>
    private async Task<string> CreateFilteredTocFileAsync(string filePath)
    {
        var psi = new ProcessStartInfo("pg_restore")
        {
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };
        psi.ArgumentList.Add("--list");
        psi.ArgumentList.Add("-Fc");
        psi.ArgumentList.Add(filePath);

        var process = StartPgTool(psi, "pg_restore");
        var output = await process.StandardOutput.ReadToEndAsync();
        await process.StandardError.ReadToEndAsync();
        await process.WaitForExitAsync();

        if (process.ExitCode != 0)
        {
            throw new BadRequestException("تعذّر قراءة محتويات ملف النسخة الاحتياطية.");
        }

        var filtered = output
            .Split('\n')
            .Where(line => !line.Contains("pg_stat_statements", StringComparison.OrdinalIgnoreCase))
            .ToList();

        var tocPath = Path.Combine(Path.GetTempPath(), $"restore_toc_{DateTime.UtcNow:yyyyMMddHHmmssfff}.list");
        await File.WriteAllLinesAsync(tocPath, filtered);
        return tocPath;
    }

    private async Task EnforceRetentionAsync(string directory)
    {
        var settings = await repository.GetSettingAsync();
        if (settings is null)
        {
            return;
        }

        var toDelete = await repository.GetOldestBeyondKeepAsync(settings.MaxBackupsToKeep, directory);
        if (toDelete.Count == 0)
        {
            return;
        }

        foreach (var backup in toDelete)
        {
            await storage.DeleteAsync(backup.FileName, backup.Directory);
            await repository.DeleteAsync(backup);
        }
        await repository.SaveChangesAsync();
        logger.LogInformation("Retention cleanup removed {Count} backup(s) from {Directory}", toDelete.Count, directory);
    }

    /// <summary>
    /// تحديد مجلد الحفظ الفعلي: المسار المطلوب (للنسخ اليدوية) ثم مجلد
    /// إعدادات النسخ التلقائي ثم إعداد التهيئة ثم سكّرينة المكتب (الافتراضي).
    /// </summary>
    private async Task<string> ResolveTargetDirectoryAsync(string? preferred)
    {
        if (!string.IsNullOrWhiteSpace(preferred))
        {
            return storage.ResolveDirectory(preferred);
        }

        var settings = await repository.GetSettingAsync();
        if (!string.IsNullOrWhiteSpace(settings?.BackupDirectory))
        {
            return storage.ResolveDirectory(settings.BackupDirectory);
        }

        var fromConfig = configuration["Backup:StorageDirectory"];
        return storage.ResolveDirectory(string.IsNullOrWhiteSpace(fromConfig) ? DefaultDirectory() : fromConfig);
    }

    private static string DefaultDirectory()
    {
        var desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        return !string.IsNullOrWhiteSpace(desktop)
            ? Path.Combine(desktop, "QuranSchool_Backups")
            : Path.Combine(Environment.CurrentDirectory, "Backups");
    }

    private async Task<DatabaseBackup> GetExistingAsync(int id)
        => await repository.GetByIdAsync(id)
           ?? throw new NotFoundException($"لا توجد نسخة احتياطية بالمعرّف {id}.");

    private async Task LogAuditAsync(string action, DatabaseBackup? backup, int? adminId, string adminName, string? details)
    {
        await repository.AddAuditAsync(new BackupAuditLog
        {
            BackupId = backup?.Id,
            BackupFileName = backup?.FileName ?? string.Empty,
            Action = action,
            PerformedById = adminId,
            PerformedByName = string.IsNullOrWhiteSpace(adminName) ? "تلقائي" : adminName,
            PerformedAt = DateTime.UtcNow,
            Details = details
        });
        await repository.SaveChangesAsync();
    }

    private (string Host, string Port, string Database, string Username, string Password, string SslMode) GetDbInfo()
    {
        var cs = configuration.GetConnectionString("DefaultConnection") ?? string.Empty;

        // يدعم صيغة URI مثل postgres://user:pass@host:5432/db?sslmode=require
        // التي تعتمدها عادةً متغيرات بيئة Render، بالإضافة إلى صيغة key=value.
        if (cs.StartsWith("postgres://", StringComparison.OrdinalIgnoreCase) ||
            cs.StartsWith("postgresql://", StringComparison.OrdinalIgnoreCase))
        {
            return ParsePostgresUri(cs);
        }

        var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        foreach (var part in cs.Split(';', StringSplitOptions.RemoveEmptyEntries))
        {
            var idx = part.IndexOf('=');
            if (idx <= 0)
            {
                continue;
            }
            map[part[..idx].Trim()] = part[(idx + 1)..].Trim();
        }

        var sslMode = map.TryGetValue("SSL Mode", out var ssl) && ssl.Length > 0
            ? ToLibpqSslMode(ssl)
            : string.Empty;

        return (
            map.TryGetValue("Host", out var host) && host.Length > 0 ? host : "localhost",
            map.TryGetValue("Port", out var port) && port.Length > 0 ? port : "5432",
            map.TryGetValue("Database", out var db) && db.Length > 0 ? db : "quran_school",
            map.TryGetValue("Username", out var user) && user.Length > 0 ? user : "postgres",
            map.TryGetValue("Password", out var password) ? password : string.Empty,
            sslMode);
    }

    private static (string Host, string Port, string Database, string Username, string Password, string SslMode) ParsePostgresUri(string cs)
    {
        try
        {
            var uri = new Uri(cs);
            var host = uri.Host.Length > 0 ? uri.Host : "localhost";
            var port = uri.IsDefaultPort ? "5432" : uri.Port.ToString();
            var database = Uri.UnescapeDataString(uri.AbsolutePath.TrimStart('/'));
            var user = "postgres";
            var password = string.Empty;
            if (!string.IsNullOrEmpty(uri.UserInfo))
            {
                var parts = uri.UserInfo.Split(':', 2);
                user = Uri.UnescapeDataString(parts[0]);
                if (parts.Length > 1)
                {
                    password = Uri.UnescapeDataString(parts[1]);
                }
            }

            var sslMode = string.Empty;
            var query = uri.Query;
            if (query.IndexOf("sslmode=require", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                sslMode = "require";
            }
            else if (query.IndexOf("sslmode=verify-full", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                sslMode = "verify-full";
            }
            else if (query.IndexOf("sslmode=verify-ca", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                sslMode = "verify-ca";
            }
            else if (query.IndexOf("sslmode=disable", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                sslMode = "disable";
            }

            return (host, port, database, user, password, sslMode);
        }
        catch
        {
            return ("localhost", "5432", "quran_school", "postgres", string.Empty, string.Empty);
        }
    }

    private static string ToLibpqSslMode(string npgsqlMode) => npgsqlMode.ToLowerInvariant() switch
    {
        "verifyca" => "verify-ca",
        "verifyfull" => "verify-full",
        _ => npgsqlMode.ToLowerInvariant()
    };

    private static void ValidateSettings(BackupSettingsDto dto)
    {
        if (dto.Frequency is not ("Daily" or "Weekly" or "Monthly"))
        {
            throw new BadRequestException("التكرار يجب أن يكون Daily أو Weekly أو Monthly.");
        }
        if (!TimeOnly.TryParse(dto.BackupTime, out _))
        {
            throw new BadRequestException("وقت النسخ الاحتياطي غير صالح (الصيغة المتوقعة HH:mm).");
        }
        if (dto.MaxBackupsToKeep is < 1 or > 100)
        {
            throw new BadRequestException("الحد الأقصى لعدد النسخ يجب أن يكون بين 1 و100.");
        }
    }

    private static DateTime ComputeNextRun(DateTime from, string frequency, string backupTime, DateTime? lastRun)
    {
        var time = TimeOnly.TryParse(backupTime, out var parsed) ? parsed : new TimeOnly(3, 0);
        var reference = frequency == "Daily" ? from : (lastRun ?? from);

        var next = reference.Date.Add(time.ToTimeSpan());
        if (next <= from)
        {
            next = next.AddDays(1);
        }

        var minIntervalDays = frequency switch
        {
            "Weekly" => 7,
            "Monthly" => 28,
            _ => 0
        };

        while ((next - reference).TotalDays < minIntervalDays)
        {
            next = next.AddDays(1);
        }

        return next;
    }

    private static DatabaseBackupDto MapToDto(DatabaseBackup b) => new()
    {
        Id = b.Id,
        FileName = b.FileName,
        Directory = b.Directory,
        FileSize = b.FileSize,
        CreatedDate = b.CreatedDate,
        BackupType = b.BackupType,
        Status = b.Status,
        CreatedByName = b.CreatedByName,
        RestoreDate = b.RestoreDate,
        RestoreStatus = b.RestoreStatus
    };

    private static BackupSettingsDto MapSettings(DatabaseBackupSetting? s) => new()
    {
        IsEnabled = s?.IsEnabled ?? true,
        Frequency = s?.Frequency ?? "Daily",
        BackupTime = s?.BackupTime ?? "03:00",
        MaxBackupsToKeep = s?.MaxBackupsToKeep ?? 10,
        BackupDirectory = s?.BackupDirectory,
        LastRunAt = s?.LastRunAt,
        NextRunAt = s?.NextRunAt
    };

    private static string Shorten(string? value, int max)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }
        return value.Length <= max ? value.Trim() : value[..max].Trim() + "…";
    }
}
