using QuranSchool.Api.DTOs.DatabaseBackup;

namespace QuranSchool.Api.Services.Interfaces;

/// <summary>
/// إدارة النسخ الاحتياطية واستعادتها لقاعدة البيانات باستخدام أدوات
/// PostgreSQL الأصلية (pg_dump / pg_restore) — مستقل عن بيئة الاستضافة.
/// </summary>
public interface IBackupService
{
    Task<List<DatabaseBackupDto>> GetBackupsAsync();
    Task<BackupSummaryDto> GetSummaryAsync();
    Task<DatabaseBackupDto> CreateBackupAsync(int adminId, string adminName, string? directory = null);
    Task<(byte[] Data, string FileName)> DownloadAsync(int backupId, int adminId, string adminName);
    Task RestoreAsync(int backupId, int adminId, string adminName);
    Task DeleteAsync(int backupId, int adminId, string adminName);
    Task<BackupSettingsDto> GetSettingsAsync();
    Task<BackupSettingsDto> UpdateSettingsAsync(BackupSettingsDto settings, int adminId, string adminName);
    Task<List<BackupAuditLogDto>> GetAuditLogsAsync(int limit = 50);
    Task RunScheduledBackupIfDueAsync();

    /// <summary>تصفح مجلدات الخادم لاختيار مسار حفظ النسخ الاحتياطية.</summary>
    DirectoryBrowserDto GetDirectoryStructure(string? path);
}
