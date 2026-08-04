using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuranSchool.Api.DTOs.Notification;
using QuranSchool.Api.Exceptions;
using QuranSchool.Api.Services.Interfaces;

namespace QuranSchool.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class NotificationsController(INotificationService service) : ControllerBase
{
    /// <summary>إرسال إشعار جديد (مع حلّ المستلمين والتحقق من الصلاحيات).</summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateNotificationRequest request)
        => Ok(await service.SendAsync(request));

    /// <summary>قائمة كاملة (للتوافق مع الواجهات السابقة).</summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
        => Ok(await service.GetAllAsync());

    /// <summary>تفاصيل إشعار مع مستلميه وحالات القراءة.</summary>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var detail = await service.GetDetailAsync(id);
        return detail is null ? NotFound() : Ok(detail);
    }

    /// <summary>تعديل إشعار (للتوافق مع الواجهات السابقة).</summary>
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] Models.AppNotification entity)
    {
        if (id != entity.Id)
        {
            return BadRequest("Route id does not match body id.");
        }

        await service.UpdateAsync(entity);
        return NoContent();
    }

    /// <summary>حذف ناعم لإشعار.</summary>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await service.DeleteAsync(id);
        return NoContent();
    }

    /// <summary>الوارد: إشعارات موجهة للمستخدم مع حالة قراءته.</summary>
    [HttpGet("inbox/{userRole}/{userId:int}")]
    public async Task<IActionResult> GetInbox(
        string userRole,
        int userId,
        [FromQuery] string? keyword,
        [FromQuery] string? category,
        [FromQuery] string? priority,
        [FromQuery] bool? unreadOnly,
        [FromQuery] bool? archived,
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to)
        => Ok(await service.GetInboxAsync(userRole, userId, new NotificationFilter
        {
            Keyword = keyword,
            Category = category,
            Priority = priority,
            UnreadOnly = unreadOnly,
            Archived = archived,
            From = from,
            To = to
        }));

    /// <summary>الإشعارات المرسلة من المستخدم مع إحصاءات القراءة.</summary>
    [HttpGet("sent/{userRole}/{userId:int}")]
    public async Task<IActionResult> GetSent(
        string userRole,
        int userId,
        [FromQuery] string? keyword,
        [FromQuery] string? category,
        [FromQuery] string? priority,
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to)
        => Ok(await service.GetSentAsync(userRole, userId, new NotificationFilter
        {
            Keyword = keyword,
            Category = category,
            Priority = priority,
            From = from,
            To = to
        }));

    /// <summary>عدد الإشعارات غير المقروءة لمستخدم.</summary>
    [HttpGet("unread-count/{userRole}/{userId:int}")]
    public async Task<IActionResult> GetUnreadCount(string userRole, int userId)
        => Ok(await service.GetUnreadCountAsync(userRole, userId));

    /// <summary>تحديد إشعار كمقروء لمستخدم معيّن.</summary>
    [HttpPut("{id:int}/read/{userRole}/{userId:int}")]
    public async Task<IActionResult> MarkRead(int id, string userRole, int userId)
    {
        await service.MarkReadAsync(id, userRole, userId);
        return NoContent();
    }

    /// <summary>أرشفة/إلغاء أرشفة إشعار في وارد المستخدم.</summary>
    [HttpPut("{id:int}/archive/{userRole}/{userId:int}")]
    public async Task<IActionResult> SetArchived(
        int id,
        string userRole,
        int userId,
        [FromQuery] bool archived = true)
    {
        await service.SetArchivedAsync(id, userRole, userId, archived);
        return NoContent();
    }

    /// <summary>قائمة مستلمي إشعار مع حالات قراءتهم (يرى المرسل من قرأ ومن لم يقرأ).</summary>
    [HttpGet("{id:int}/recipients")]
    public async Task<IActionResult> GetRecipients(int id)
        => Ok(await service.GetRecipientsAsync(id));

    /// <summary>بحث في الوارد والمرسل حسب الكلمة والفلاتر.</summary>
    [HttpGet("search/{userRole}/{userId:int}")]
    public async Task<IActionResult> Search(
        string userRole,
        int userId,
        [FromQuery] string? keyword,
        [FromQuery] string? category,
        [FromQuery] string? priority,
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to)
        => Ok(await service.SearchAsync(userRole, userId, new NotificationFilter
        {
            Keyword = keyword,
            Category = category,
            Priority = priority,
            From = from,
            To = to
        }));

    /// <summary>الإشعارات المرسلة (متوافق قديمًا مع الواجهات السابقة).</summary>
    [HttpGet("by-sender/{senderType}/{senderId:int}")]
    public async Task<IActionResult> GetBySender(string senderType, int senderId)
        => Ok(await service.GetBySenderAsync(senderType, senderId));

    [HttpGet("latest/{count:int}")]
    public async Task<IActionResult> GetLatest(int count)
        => Ok(await service.GetLatestAsync(count));

    /// <summary>قائمة الإداريين لاختيار المستلمين (متاحة لكل الأدوار المسجّلة).</summary>
    [HttpGet("admin-users")]
    public async Task<IActionResult> GetAdminUsers()
        => Ok(await service.GetAdminUsersAsync());
}
