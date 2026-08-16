namespace QuranSchool.Api.Services.Interfaces;

/// <summary>
/// تجريد مساحة تخزين ملفات النسخ الاحتياطية — مستقل عن مزوّد الاستضافة.
/// الإضافة الافتراضية هي التخزين المحلي على القرص، ويمكن إضافة مزوّدات
/// أخرى (Google Drive, AWS S3, Azure Blob, FTP...) بتنفيذ جديد فقط دون
/// تغيير منطق النسخ الاحتياطي.
/// </summary>
public interface IBackupStorage
{
    /// <summary>المسار/الحاوية الافتراضية التي تُخزَّن فيها ملفات النسخ الاحتياطية.</summary>
    string DirectoryPath { get; }

    /// <summary>
    /// تحويل مجلد مُدخَل إلى مسار كامل صالح (المسار المطلق يُستخدم كما هو،
    /// والمسار النسبي يُحسب من مجلد عمل التطبيق). عند تمرير null يُستخدم
    /// المجلد الافتراضي. يُنشئ المجلد إذا لم يكن موجودًا.
    /// </summary>
    string ResolveDirectory(string? directory = null);

    /// <summary>إرجاع المسار الكامل لملف داخل المجلد المعطى (أو الافتراضي).</summary>
    string GetAbsolutePath(string fileName, string? directory = null);

    /// <summary>قراءة محتوى ملف نسخة احتياطية.</summary>
    Task<byte[]> ReadAsync(string fileName, string? directory = null);

    /// <summary>حذف ملف نسخة احتياطية (لا يرمي خطأً إذا لم يوجد).</summary>
    Task DeleteAsync(string fileName, string? directory = null);

    /// <summary>حجم الملف بالبايت.</summary>
    Task<long> GetSizeAsync(string fileName, string? directory = null);
}
