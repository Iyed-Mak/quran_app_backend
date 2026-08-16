using QuranSchool.Api.Services.Interfaces;

namespace QuranSchool.Api.Services.Implementations;

/// <summary>
/// تخزين ملفات النسخ الاحتياطية في مجلد محلي على قرص الخادم.
/// المجلد الافتراضي قابل للتهيئة عبر الإعداد <c>Backup:StorageDirectory</c> في
/// appsettings.json أو متغير البيئة <c>Backup__StorageDirectory</c>.
/// المسار النسبي يُحسب انطلاقًا من مجلد عمل التطبيق الحالي، والمسار
/// المطلق يُستخدم كما هو. يمكن لكل عملية تحديد مجلد مختلف عبر معامل
/// <c>directory</c>.
/// </summary>
public class LocalBackupStorage(IConfiguration configuration) : IBackupStorage
{
    private readonly string _defaultDirectory = Resolve(configuration["Backup:StorageDirectory"] ?? "Backups");

    public string DirectoryPath => _defaultDirectory;

    public string ResolveDirectory(string? directory = null)
    {
        var path = string.IsNullOrWhiteSpace(directory) ? _defaultDirectory : directory.Trim();
        if (string.IsNullOrWhiteSpace(path))
        {
            path = _defaultDirectory;
        }
        if (!Path.IsPathRooted(path))
        {
            path = Path.Combine(Environment.CurrentDirectory, path);
        }
        Directory.CreateDirectory(path);
        return path;
    }

    public string GetAbsolutePath(string fileName, string? directory = null)
        => Path.Combine(ResolveDirectory(directory), fileName);

    public Task<byte[]> ReadAsync(string fileName, string? directory = null)
    {
        var path = GetAbsolutePath(fileName, directory);
        if (!File.Exists(path))
        {
            throw new FileNotFoundException($"الملف {fileName} غير موجود على الخادم.", path);
        }
        return File.ReadAllBytesAsync(path);
    }

    public Task DeleteAsync(string fileName, string? directory = null)
    {
        var path = GetAbsolutePath(fileName, directory);
        if (File.Exists(path))
        {
            File.Delete(path);
        }
        return Task.CompletedTask;
    }

    public Task<long> GetSizeAsync(string fileName, string? directory = null)
    {
        var path = GetAbsolutePath(fileName, directory);
        return Task.FromResult(File.Exists(path) ? new FileInfo(path).Length : 0L);
    }

    private static string Resolve(string configured)
    {
        var path = configured.Trim();
        if (string.IsNullOrWhiteSpace(path))
        {
            path = "Backups";
        }
        if (!Path.IsPathRooted(path))
        {
            path = Path.Combine(Environment.CurrentDirectory, path);
        }
        Directory.CreateDirectory(path);
        return path;
    }
}
