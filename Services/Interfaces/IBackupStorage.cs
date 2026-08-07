namespace QuranSchool.Api.Services.Interfaces;

/// <summary>
/// تجريد مساحة تخزين ملفات النسخ الاحتياطية — مستقل عن مزوّد الاستضافة.
/// الإضافة الافتراضية هي التخزين المحلي على القرص، ويمكن إضافة مزوّدات
/// أخرى (Google Drive, AWS S3, Azure Blob, FTP...) بتنفيذ جديد فقط دون
/// تغيير منطق النسخ الاحتياطي.
/// </summary>
public interface IBackupStorage
{
    /// <summary>المسار/الحاوية التي تُخزَّن فيها ملفات النسخ الاحتياطية.</summary>
    string DirectoryPath { get; }

    /// <summary>إرجاع المسار الكامل لملف داخل مساحة التخزين.</summary>
    string GetAbsolutePath(string fileName);

    /// <summary>قراءة محتوى ملف نسخة احتياطية.</summary>
    Task<byte[]> ReadAsync(string fileName);

    /// <summary>حذف ملف نسخة احتياطية (لا يرمي خطأً إذا لم يوجد).</summary>
    Task DeleteAsync(string fileName);

    /// <summary>حجم الملف بالبايت.</summary>
    Task<long> GetSizeAsync(string fileName);
}
