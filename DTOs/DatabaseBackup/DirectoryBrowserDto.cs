namespace QuranSchool.Api.DTOs.DatabaseBackup;

/// <summary>
/// نتيجة تصفح المجلدات على الخادم لاختيار مسار حفظ النسخ الاحتياطية.
/// </summary>
public class DirectoryBrowserDto
{
    public string CurrentPath { get; set; } = string.Empty;

    /// <summary>المجلد الأب (null إذا كنا في جذر نظام الملفات).</summary>
    public string? ParentPath { get; set; }

    public bool IsRoot { get; set; }

    /// <summary>المجلدات الفرعية داخل [CurrentPath] (مرتبة أبجديًا).</summary>
    public List<DirectoryEntryDto> Entries { get; set; } = new();
}

public class DirectoryEntryDto
{
    public string Name { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
}
