using Microsoft.EntityFrameworkCore;
using QuranSchool.Api.Data;
using QuranSchool.Api.Models;
using QuranSchool.Api.Repositories.Interfaces;

namespace QuranSchool.Api.Repositories.Implementations;

public class DatabaseBackupRepository(AppDbContext context) : IDatabaseBackupRepository
{
    public async Task<List<DatabaseBackup>> GetAllAsync()
        => await context.DatabaseBackups
            .OrderByDescending(b => b.CreatedDate)
            .AsNoTracking()
            .ToListAsync();

    public async Task<DatabaseBackup?> GetByIdAsync(int id)
        => await context.DatabaseBackups
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.Id == id);

    public async Task AddAsync(DatabaseBackup backup)
        => await context.DatabaseBackups.AddAsync(backup);

    public Task DeleteAsync(DatabaseBackup backup)
    {
        context.DatabaseBackups.Remove(backup);
        return Task.CompletedTask;
    }

    public async Task<DatabaseBackup?> GetLatestAsync()
        => await context.DatabaseBackups
            .OrderByDescending(b => b.CreatedDate)
            .AsNoTracking()
            .FirstOrDefaultAsync();

    public async Task<List<DatabaseBackup>> GetOldestBeyondKeepAsync(int keep, string directory)
        => await context.DatabaseBackups
            .Where(b => b.Directory == directory)
            .OrderByDescending(b => b.CreatedDate)
            .Skip(Math.Max(0, keep))
            .AsNoTracking()
            .ToListAsync();

    public async Task SaveChangesAsync() => await context.SaveChangesAsync();

    public async Task<DatabaseBackupSetting?> GetSettingAsync()
        => await context.DatabaseBackupSettings
            .AsNoTracking()
            .FirstOrDefaultAsync();

    public async Task SaveSettingAsync(DatabaseBackupSetting setting)
    {
        var existing = await context.DatabaseBackupSettings.FirstOrDefaultAsync();
        if (existing is null)
        {
            context.DatabaseBackupSettings.Add(setting);
        }
        else
        {
            setting.Id = existing.Id;
            context.Entry(existing).CurrentValues.SetValues(setting);
        }
    }

    public async Task AddAuditAsync(BackupAuditLog log)
        => await context.BackupAuditLogs.AddAsync(log);

    public async Task<List<BackupAuditLog>> GetAuditLogsAsync(int limit)
        => await context.BackupAuditLogs
            .OrderByDescending(l => l.PerformedAt)
            .Take(Math.Clamp(limit, 1, 200))
            .AsNoTracking()
            .ToListAsync();
}
