namespace QuranSchool.Api.DTOs.DatabaseBackup;

public class DatabaseBackupDto
{
    public int Id { get; set; }
    public string FileName { get; set; } = string.Empty;

    /// <summary>المجلد الذي حُفظ فيه الملف على وحدة التخزين.</summary>
    public string Directory { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public DateTime CreatedDate { get; set; }
    public string BackupType { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string CreatedByName { get; set; } = string.Empty;
    public DateTime? RestoreDate { get; set; }
    public string? RestoreStatus { get; set; }
}
