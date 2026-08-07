namespace QuranSchool.Api.DTOs.DatabaseBackup;

public class BackupSettingsDto
{
    public bool IsEnabled { get; set; } = true;

    /// <summary>التكرار: Daily / Weekly / Monthly.</summary>
    public string Frequency { get; set; } = "Daily";

    /// <summary>وقت النسخ بصيغة HH:mm.</summary>
    public string BackupTime { get; set; } = "03:00";

    /// <summary>الحد الأقصى لعدد النسخ المحتفظ بها.</summary>
    public int MaxBackupsToKeep { get; set; } = 10;

    public DateTime? LastRunAt { get; set; }
    public DateTime? NextRunAt { get; set; }
}
