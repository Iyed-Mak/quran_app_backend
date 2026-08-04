using System.ComponentModel.DataAnnotations.Schema;

namespace QuranSchool.Api.Models;

[Table("notification")]
public class AppNotification : IEntity
{
    public int Id { get; set; }
    public string SenderType { get; set; } = string.Empty;
    public int SenderId { get; set; }
    public string SenderName { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Priority { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string RecipientLabel { get; set; } = string.Empty;

    /// <summary>مسار مرفق اختياري (PDF/صورة/Word) مخزّن على الخادم.</summary>
    public string? AttachmentPath { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    /// <summary>حذف ناعم: تبقى السجلات لحفظ السجلّات والإحصائيات.</summary>
    public bool IsDeleted { get; set; }

    public List<NotificationReceiver> Receivers { get; set; } = new();
}
