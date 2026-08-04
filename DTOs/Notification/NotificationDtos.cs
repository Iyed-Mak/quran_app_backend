namespace QuranSchool.Api.DTOs.Notification;

/// <summary>مستلم محدد (دور + معرّف مستخدم) في طلب إرسال إشعار.</summary>
public class RecipientUserDto
{
    /// <summary>'admin' | 'teacher' | 'student' | 'parent'</summary>
    public string Role { get; set; } = string.Empty;
    public int UserId { get; set; }
}

/// <summary>
/// هدف واحد للمستلمين. يمكن أن يجمع دورًا كاملًا (Role) أو أفواجًا
/// (GroupIds) أو أفرادًا محددين (Users) — أيّها كان معبّأً.
/// </summary>
public class RecipientTargetDto
{
    /// <summary>
    /// 'all' | 'teachers' | 'students' | 'admins' | 'parents' | null.
    /// عندما يكون null نستخدم GroupIds أو Users.
    /// </summary>
    public string? Role { get; set; }
    public List<int>? GroupIds { get; set; }
    public List<RecipientUserDto>? Users { get; set; }
}

/// <summary>طلب إنشاء/إرسال إشعار جديد.</summary>
public class CreateNotificationRequest
{
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;

    /// <summary>فئة الإشعار: عام/واجب/حضور/امتحان/جدول/إداري/تنبيه/مالي/أخرى...</summary>
    public string Category { get; set; } = "عام";

    /// <summary>أولوية: منخفضة/عادية/عالية/عاجلة</summary>
    public string Priority { get; set; } = "عادية";

    /// <summary>مسار مرفق اختياري (يُرفع سابقًا ويُمرَّر المسار فقط).</summary>
    public string? AttachmentPath { get; set; }

    /// <summary>هوية المرسل (يُتحقق منها مع هوية التوكن).</summary>
    public int SenderId { get; set; }
    public string SenderRole { get; set; } = string.Empty;
    public string SenderName { get; set; } = string.Empty;

    /// <summary>أهداف المستلمين (يمكن دمج أكثر من هدف في إشعار واحد).</summary>
    public List<RecipientTargetDto> Recipients { get; set; } = new();
}

/// <summary>بيانات الإشعار الأساسية (للعرض).</summary>
public class NotificationDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Priority { get; set; } = string.Empty;
    public string? AttachmentPath { get; set; }
    public string Status { get; set; } = string.Empty;
    public string RecipientLabel { get; set; } = string.Empty;
    public string SenderRole { get; set; } = string.Empty;
    public int SenderId { get; set; }
    public string SenderName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }
}

/// <summary>عنصر الوارد: إشعار + حالة الاستلام الخاصة بالمستخدم الحالي.</summary>
public class NotificationInboxItemDto : NotificationDto
{
    public bool IsRead { get; set; }
    public DateTime? ReadAt { get; set; }
    public bool IsArchived { get; set; }
    public int ReceiverId { get; set; }
}

/// <summary>عنصر المرسل: يظهر للمرسل مع ملخص عدد المستلمين والمقروءين.</summary>
public class NotificationSentItemDto : NotificationDto
{
    public int RecipientsCount { get; set; }
    public int ReadCount { get; set; }
}

/// <summary>مستلم واحد مع حالة قراءته (يراه المرسل في تفاصيل الإشعار).</summary>
public class RecipientDto
{
    public int Id { get; set; }
    public int NotificationId { get; set; }
    public string ReceiverRole { get; set; } = string.Empty;
    public int ReceiverId { get; set; }
    public string ReceiverName { get; set; } = string.Empty;
    public bool IsRead { get; set; }
    public DateTime? ReadAt { get; set; }
    public bool IsArchived { get; set; }
}

/// <summary>تفاصيل الإشعار الكاملة: بياناته + قائمة مستلميه مع حالات القراءة.</summary>
public class NotificationDetailDto : NotificationDto
{
    public List<RecipientDto> Recipients { get; set; } = new();
}

/// <summary>عوامل ترشيح الوارد/المرسل/البحث.</summary>
public class NotificationFilter
{
    public string? Keyword { get; set; }
    public string? Category { get; set; }
    public string? Priority { get; set; }
    public bool? UnreadOnly { get; set; }
    public bool? Archived { get; set; }
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 100;
}

/// <summary>نتيجة بحث مجمّعة.</summary>
public class NotificationSearchResultDto
{
    public List<NotificationDetailDto> Items { get; set; } = new();
    public int Total { get; set; }
}

/// <summary>
/// بيانات إداري مصغّرة لاختيار المستلمين في واجهة الإرسال
/// (بدون بيانات حساسة مثل كلمة المرور)، متاحة لجميع الأدوار المسجّلة.
/// </summary>
public class AdminUserDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}
