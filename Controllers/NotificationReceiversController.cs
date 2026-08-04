using Microsoft.AspNetCore.Mvc;
using QuranSchool.Api.Models;
using QuranSchool.Api.Services.Interfaces;

namespace QuranSchool.Api.Controllers;

[Route("api/[controller]")]
public class NotificationReceiversController : BaseCrudController<NotificationReceiver, INotificationReceiverService>
{
    public NotificationReceiversController(INotificationReceiverService service) : base(service)
    {
    }

    [HttpGet("by-user/{userType}/{userId:int}")]
    public async Task<IActionResult> GetByUser(string userType, int userId)
        => Ok(await _service.GetByUserAsync(userType, userId));

    [HttpGet("unread-count/{userType}/{userId:int}")]
    public async Task<IActionResult> GetUnreadCount(string userType, int userId)
        => Ok(await _service.GetUnreadCountAsync(userType, userId));
}
