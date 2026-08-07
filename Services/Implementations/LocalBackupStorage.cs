using QuranSchool.Api.Services.Interfaces;

namespace QuranSchool.Api.Services.Implementations;

/// <summary>
/// تخزين ملفات النسخ الاحتياطية في مجلد محلي على قرص الخادم.
/// المجلد قابل للتهيئة عبر الإعداد <c>Backup:StorageDirectory</c> في
/// appsettings.json أو متغير البيئة <c>Backup__StorageDirectory</c>.
/// المسار النسبي يُحسب انطلاقًا من مجلد عمل التطبيق الحالي، والمسار
/// المطلق يُستخدم كما هو.
/// </summary>
public class LocalBackupStorage(IConfiguration configuration) : IBackupStorage
{
    private readonly string _directory = ResolveDirectory(
        configuration["Backup:StorageDirectory"] ?? "Backups");

    public string DirectoryPath => _directory;

    public string GetAbsolutePath(string fileName)
        => Path.Combine(_directory, fileName);

    public Task<byte[]> ReadAsync(string fileName)
    {
        var path = GetAbsolutePath(fileName);
        if (!File.Exists(path))
        {
            throw new FileNotFoundException($"الملف {fileName} غير موجود على الخادم.", path);
        }
        return File.ReadAllBytesAsync(path);
    }

    public Task DeleteAsync(string fileName)
    {
        var path = GetAbsolutePath(fileName);
        if (File.Exists(path))
        {
            File.Delete(path);
        }
        return Task.CompletedTask;
    }

    public Task<long> GetSizeAsync(string fileName)
    {
        var path = GetAbsolutePath(fileName);
        return Task.FromResult(File.Exists(path) ? new FileInfo(path).Length : 0L);
    }

    private static string ResolveDirectory(string configured)
    {
        var path = configured.Trim();
        if (string.IsNullOrWhiteSpace(path))
        {
            path = "Backups";
        }
        if (!Path.IsPathRooted(path))
        {
            path = Path.Combine(Environment.CurrentDirectory, path);
        }
        Directory.CreateDirectory(path);
        return path;
    }
}
