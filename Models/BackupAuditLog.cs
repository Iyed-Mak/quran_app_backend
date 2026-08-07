using System.ComponentModel.DataAnnotations.Schema;

namespace QuranSchool.Api.Models;

/// <summary>
/// سجل تدقيق لكل عملية نسخ/استعادة/تنزيل/حذف تتعلق بالنسخ الاحتياطية.
/// </summary>
[Table("backup_audit_logs")]
public class BackupAuditLog : IEntity
{
    public int Id { get; set; }

    /// <summary>معرّف النسخة الاحتياطية المرتبطة (إن وُجد).</summary>
    public int? BackupId { get; set; }

    public string BackupFileName { get; set; } = string.Empty;

    /// <summary>الإجراء: Create / Restore / Download / Delete / UpdateSettings.</summary>
    public string Action { get; set; } = string.Empty;

    public int? PerformedById { get; set; }

    public string PerformedByName { get; set; } = string.Empty;

    public DateTime PerformedAt { get; set; }

    public string? Details { get; set; }
}
