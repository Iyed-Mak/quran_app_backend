using System.ComponentModel.DataAnnotations.Schema;

namespace QuranSchool.Api.Models;

/// <summary>
/// إعدادات النسخ الاحتياطي التلقائي (صف واحد).
/// </summary>
[Table("database_backup_settings")]
public class DatabaseBackupSetting : IEntity
{
    public int Id { get; set; }

    /// <summary>تفعيل/تعطيل النسخ الاحتياطي التلقائي.</summary>
    public bool IsEnabled { get; set; } = true;

    /// <summary>التكرار: Daily / Weekly / Monthly.</summary>
    public string Frequency { get; set; } = "Daily";

    /// <summary>وقت النسخ بصيغة HH:mm.</summary>
    public string BackupTime { get; set; } = "03:00";

    /// <summary>الحد الأقصى لعدد النسخ الاحتياطية المحتفظ بها.</summary>
    public int MaxBackupsToKeep { get; set; } = 10;

    /// <summary>آخر مرة نُفّذ فيها نسخ تلقائي (UTC).</summary>
    public DateTime? LastRunAt { get; set; }

    /// <summary>الموعد التالي المقرر للنسخ التلقائي (UTC).</summary>
    public DateTime? NextRunAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    /// <summary>معرّف المسؤول الذي عدّل الإعدادات آخر مرة.</summary>
    public int UpdatedBy { get; set; }
}
