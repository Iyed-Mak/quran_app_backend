using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace QuranSchool.Api.Models;

[Table("notification_receiver")]
public class NotificationReceiver : IEntity
{
    public int Id { get; set; }
    public int NotificationId { get; set; }
    public string UserType { get; set; } = string.Empty;
    public int UserId { get; set; }
    public string UserFullName { get; set; } = string.Empty;
    public bool IsRead { get; set; }
    public DateTime? ReadAt { get; set; }

    /// <summary>أرشفة من جهة المستلم (تبقى في الوارد لكن بأرشفة).</summary>
    public bool IsArchived { get; set; }

    [ValidateNever]
    public AppNotification Notification { get; set; } = null!;
}
