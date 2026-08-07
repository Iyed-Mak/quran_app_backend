using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuranSchool.Api.DTOs.DatabaseBackup;
using QuranSchool.Api.Services.Interfaces;

namespace QuranSchool.Api.Controllers;

/// <summary>
/// إدارة النسخ الاحتياطي واستعادة قاعدة البيانات — متاحة لجميع
/// حسابات المسؤولين (Admin).
/// </summary>
[ApiController]
[Authorize]
[Route("api/[controller]")]
public class DatabaseBackupsController(IBackupService service) : ControllerBase
{
    /// <summary>قائمة النسخ الاحتياطية (الأحدث أولًا).</summary>
    [HttpGet]

    public async Task<IActionResult> GetAll()
        => Ok(await service.GetBackupsAsync());

    /// <summary>ملخص حالة النسخ الاحتياطي (آخر نسخة، موعد التالي، الإجمالي...).</summary>
    [HttpGet("summary")]

    public async Task<IActionResult> GetSummary()
        => Ok(await service.GetSummaryAsync());

    /// <summary>إنشاء نسخة احتياطية يدوية الآن.</summary>
    [HttpPost]

    public async Task<IActionResult> Create()
        => Ok(await service.CreateBackupAsync(GetUserId(), GetUsername()));

    /// <summary>تنزيل ملف النسخة الاحتياطية.</summary>
    [HttpGet("{id:int}/download")]

    public async Task<IActionResult> Download(int id)
    {
        var (data, fileName) = await service.DownloadAsync(id, GetUserId(), GetUsername());
        return File(data, "application/octet-stream", fileName);
    }

    /// <summary>استعادة قاعدة البيانات من نسخة احتياطية.</summary>
    [HttpPost("{id:int}/restore")]

    public async Task<IActionResult> Restore(int id)
    {
        await service.RestoreAsync(id, GetUserId(), GetUsername());
        return NoContent();
    }

    /// <summary>حذف نسخة احتياطية وملفها.</summary>
    [HttpDelete("{id:int}")]

    public async Task<IActionResult> Delete(int id)
    {
        await service.DeleteAsync(id, GetUserId(), GetUsername());
        return NoContent();
    }

    /// <summary>إعدادات النسخ الاحتياطي التلقائي.</summary>
    [HttpGet("settings")]

    public async Task<IActionResult> GetSettings()
        => Ok(await service.GetSettingsAsync());

    /// <summary>تحديث إعدادات النسخ الاحتياطي التلقائي.</summary>
    [HttpPut("settings")]

    public async Task<IActionResult> UpdateSettings([FromBody] BackupSettingsDto settings)
        => Ok(await service.UpdateSettingsAsync(settings, GetUserId(), GetUsername()));

    /// <summary>سجل التدقيق لأحداث النسخ والاستعادة والتنزيل والحذف.</summary>
    [HttpGet("audit")]

    public async Task<IActionResult> GetAuditLogs([FromQuery] int limit = 50)
        => Ok(await service.GetAuditLogsAsync(limit));

    private int GetUserId()
        => int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : 0;

    private string GetUsername()
        => User.Identity?.Name ?? string.Empty;
}
