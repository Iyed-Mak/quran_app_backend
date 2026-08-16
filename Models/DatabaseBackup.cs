using System.ComponentModel.DataAnnotations.Schema;

namespace QuranSchool.Api.Models;

/// <summary>
/// سجل نسخة احتياطية واحدة من قاعدة البيانات.
/// </summary>
[Table("database_backups")]
public class DatabaseBackup : IEntity
{
    public int Id { get; set; }

    /// <summary>اسم ملف النسخة الاحتياطية (quran_school_yyyy-MM-dd_HH-mm.backup).</summary>
    public string FileName { get; set; } = string.Empty;

    /// <summary>المسار الكامل للملف على وحدة التخزين.</summary>
    public string FilePath { get; set; } = string.Empty;

    /// <summary>المجلد الذي حُفظ فيه الملف على وحدة التخزين.</summary>
    public string Directory { get; set; } = string.Empty;

    /// <summary>حجم ملف النسخة بالبايت.</summary>
    public long FileSize { get; set; }

    /// <summary>تاريخ إنشاء النسخة (UTC).</summary>
    public DateTime CreatedDate { get; set; }

    /// <summary>نوع النسخة: Manual / Automatic.</summary>
    public string BackupType { get; set; } = "Manual";

    /// <summary>حالة النسخة: Success / Failed.</summary>
    public string Status { get; set; } = "Success";

    /// <summary>معرّف المسؤول الذي أنشأ النسخة (null للنسخ التلقائية).</summary>
    public int? CreatedBy { get; set; }

    /// <summary>اسم المستخدم الذي أنشأ النسخة ("تلقائي" للنسخ التلقائية).</summary>
    public string CreatedByName { get; set; } = string.Empty;

    /// <summary>تاريخ آخر عملية استعادة لهذه النسخة (إن وُجدت).</summary>
    public DateTime? RestoreDate { get; set; }

    /// <summary>معرّف المسؤول الذي نفّذ عملية الاستعادة.</summary>
    public int? RestoredBy { get; set; }

    /// <summary>حالة آخر عملية استعادة: Success / Failed.</summary>
    public string? RestoreStatus { get; set; }
}
