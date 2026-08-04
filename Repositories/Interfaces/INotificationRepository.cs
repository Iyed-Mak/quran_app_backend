using QuranSchool.Api.DTOs.Notification;
using QuranSchool.Api.Models;

namespace QuranSchool.Api.Repositories.Interfaces;

public interface INotificationRepository : IRepository<AppNotification>
{
    Task<List<AppNotification>> GetBySenderAsync(string senderType, int senderId);
    Task<List<AppNotification>> GetLatestAsync(int count);
    Task<AppNotification?> GetByIdWithReceiversAsync(int id);

    /// <summary>إشعارات الوارد لمستخدم معيّن (يُمرَّر قائمة معرّفات الإشعارات
    /// المستخرجة من سجلّات الاستلام).</summary>
    Task<List<AppNotification>> GetInboxNotificationsAsync(
        List<int> notificationIds,
        NotificationFilter? filter);

    /// <summary>الإشعارات المرسلة مع عوامل الترشيح.</summary>
    Task<List<AppNotification>> GetSentNotificationsAsync(
        string senderType,
        int senderId,
        NotificationFilter? filter);

    Task<List<NotificationReceiver>> GetReceiversForNotificationAsync(int notificationId);
}
