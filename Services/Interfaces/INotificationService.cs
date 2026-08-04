using QuranSchool.Api.DTOs.Notification;
using QuranSchool.Api.Models;

namespace QuranSchool.Api.Services.Interfaces;

public interface INotificationService : IService<AppNotification>
{
    Task<List<AppNotification>> GetBySenderAsync(string senderType, int senderId);
    Task<List<AppNotification>> GetLatestAsync(int count);

    /// <summary>إنشاء إشعار مع حلّ أهداف المستلمين والتحقق من صلاحيات المرسل.</summary>
    Task<AppNotification> SendAsync(CreateNotificationRequest request);

    /// <summary>تفاصيل إشعار كاملة (مع المستلمين وحالات القراءة).</summary>
    Task<NotificationDetailDto?> GetDetailAsync(int id);

    /// <summary>وارد المستخدم (الإشعارات الموجهة إليه مع حالة قراءته).</summary>
    Task<List<NotificationInboxItemDto>> GetInboxAsync(
        string role, int userId, NotificationFilter? filter);

    /// <summary>الإشعارات التي أرسلها المستخدم (مع إحصاءات القراءة).</summary>
    Task<List<NotificationSentItemDto>> GetSentAsync(
        string role, int userId, NotificationFilter? filter);

    /// <summary>عدد غير المقروء لمستخدم.</summary>
    Task<int> GetUnreadCountAsync(string role, int userId);

    /// <summary>تحديد إشعار كمقروء لمستخدم معيّن.</summary>
    Task MarkReadAsync(int notificationId, string role, int userId);

    /// <summary>أرشفة/إلغاء أرشفة إشعار في وارد المستخدم.</summary>
    Task SetArchivedAsync(int notificationId, string role, int userId, bool archived);

    /// <summary>مستلمو إشعار مع حالات قراءتهم (للمرسل).</summary>
    Task<List<RecipientDto>> GetRecipientsAsync(int notificationId);

    /// <summary>بحث في الوارد والمرسل حسب الكلمة والفلاتر.</summary>
    Task<NotificationSearchResultDto> SearchAsync(
        string role, int userId, NotificationFilter? filter);

    /// <summary>قائمة الإداريين (معرّف + اسم) لاختيار مستلمين عند الإرسال.</summary>
    Task<List<AdminUserDto>> GetAdminUsersAsync();
}
