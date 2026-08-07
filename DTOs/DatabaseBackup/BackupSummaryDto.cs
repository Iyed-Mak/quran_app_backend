namespace QuranSchool.Api.DTOs.DatabaseBackup;

public class BackupSummaryDto
{
    public DateTime? LastBackupDate { get; set; }
    public string? LastBackupFileName { get; set; }
    public DateTime? NextScheduledBackup { get; set; }
    public bool AutomaticBackupEnabled { get; set; }
    public int TotalBackups { get; set; }
    public long TotalSize { get; set; }
    public string BackupDirectory { get; set; } = string.Empty;
}
