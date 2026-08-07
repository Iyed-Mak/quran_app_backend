namespace QuranSchool.Api.DTOs.DatabaseBackup;

public class BackupAuditLogDto
{
    public int Id { get; set; }
    public int? BackupId { get; set; }
    public string BackupFileName { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string PerformedByName { get; set; } = string.Empty;
    public DateTime PerformedAt { get; set; }
    public string? Details { get; set; }
}
