namespace QuranSchool.Api.DTOs.DatabaseBackup;

/// <summary>
/// طلب إنشاء نسخة احتياطية يدوية. [Directory] اختياري: المجلد الذي تُحفظ
/// فيه النسخة (فارغ أو null = مجلد الحفظ الافتراضي من إعدادات النسخ التلقائي).
/// </summary>
public class CreateBackupRequest
{
    public string? Directory { get; set; }
}
