using QuranSchool.Api.Models;

namespace QuranSchool.Api.Repositories.Interfaces;

/// <summary>
/// مستودع سجلات النسخ الاحتياطية وإعداداتها وسجل التدقيق.
/// </summary>
public interface IDatabaseBackupRepository
{
    Task<List<DatabaseBackup>> GetAllAsync();
    Task<DatabaseBackup?> GetByIdAsync(int id);
    Task AddAsync(DatabaseBackup backup);
    Task DeleteAsync(DatabaseBackup backup);
    Task<DatabaseBackup?> GetLatestAsync();
    Task<List<DatabaseBackup>> GetOldestBeyondKeepAsync(int keep);
    Task SaveChangesAsync();

    Task<DatabaseBackupSetting?> GetSettingAsync();
    Task SaveSettingAsync(DatabaseBackupSetting setting);

    Task AddAuditAsync(BackupAuditLog log);
    Task<List<BackupAuditLog>> GetAuditLogsAsync(int limit);
}
