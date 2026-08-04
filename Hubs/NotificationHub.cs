using Microsoft.AspNetCore.SignalR;
using QuranSchool.Api.DTOs.Notification;

namespace QuranSchool.Api.Hubs;

/// <summary>عقد العميل المستقبِل عبر SignalR.</summary>
public interface INotificationClient
{
    /// <summary>وصل إشعار جديد للمستخدم.</summary>
    Task NotificationReceived(NotificationInboxItemDto notification);

    /// <summary>تغيّر عدد الإشعارات غير المقروءة.</summary>
    Task UnreadCountChanged(int unreadCount);
}

/// <summary>
/// مركز (Hub) إشعارات SignalR. يلتحق كل مستخدم بمجموعته الشخصية
/// `user:{role}:{userId}` (عبر استعلام الرابط أو دالة JoinUser) ليستقبل
/// الإشعارات لحظيًا دون إعادة تحميل.
/// </summary>
public class NotificationHub : Hub<INotificationClient>
{
    public const string HubRoute = "/hubs/notifications";

    /// <summary>تسمية مجموعة مستخدم واحد.</summary>
    public static string UserGroup(string role, int userId) => $"user:{role}:{userId}";

    /// <summary>تسمية مجموعة دور كامل (تُستخدم للإرسال الجماعي).</summary>
    public static string RoleGroup(string role) => $"role:{role}";

    /// <summary>ينضم العميل لمجموعته الشخصية بوضوح (اختياري إذا لم يمرر معرّفاته في الرابط).</summary>
    public async Task JoinUser(string role, int userId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, UserGroup(role, userId));
    }

    public override async Task OnConnectedAsync()
    {
        var httpContext = Context.GetHttpContext();
        var role = httpContext?.Request.Query["role"].ToString();
        var userIdRaw = httpContext?.Request.Query["userId"].ToString();
        if (!string.IsNullOrEmpty(role) && int.TryParse(userIdRaw, out var userId))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, UserGroup(role, userId));
            await Groups.AddToGroupAsync(Context.ConnectionId, RoleGroup(role));
        }

        await base.OnConnectedAsync();
    }
}
