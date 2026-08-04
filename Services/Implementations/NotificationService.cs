using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using QuranSchool.Api.Data;
using QuranSchool.Api.DTOs.Notification;
using QuranSchool.Api.Exceptions;
using QuranSchool.Api.Hubs;
using QuranSchool.Api.Models;
using QuranSchool.Api.Repositories.Interfaces;
using QuranSchool.Api.Services.Interfaces;

namespace QuranSchool.Api.Services.Implementations;

/// <summary>
/// خدمة الإشعارات: إنشاء الإشعارات مع حلّ المستلمين والتحقق من صلاحيات
/// الإرسال، ووارد/مرسل/قراءة/أرشفة/بحث، والبث الفوري عبر SignalR.
/// </summary>
public class NotificationService(
    INotificationRepository repository,
    AppDbContext context,
    IMapper mapper,
    IHubContext<NotificationHub, INotificationClient> hub)
    : Service<AppNotification>(repository), INotificationService
{
    public async Task<List<AppNotification>> GetBySenderAsync(string senderType, int senderId)
        => await repository.GetBySenderAsync(senderType, senderId);

    public async Task<List<AppNotification>> GetLatestAsync(int count)
        => await repository.GetLatestAsync(count);

    // ─────────────────────────────────────────────────────────────
    // إنشاء / إرسال
    // ─────────────────────────────────────────────────────────────

    public async Task<AppNotification> SendAsync(CreateNotificationRequest request)
    {
        Validate(request);

        var recipients = await ResolveRecipientsAsync(request);

        var now = DateTime.UtcNow;

        var notification = new AppNotification
        {
            SenderType = request.SenderRole,
            SenderId = request.SenderId,
            SenderName = request.SenderName,
            Title = request.Title.Trim(),
            Message = request.Message.Trim(),
            Priority = request.Priority,
            Category = request.Category,
            Status = "مرسل",
            RecipientLabel = BuildRecipientLabel(request),
            AttachmentPath = string.IsNullOrWhiteSpace(request.AttachmentPath)
                ? null
                : request.AttachmentPath.Trim(),
            CreatedAt = now,
            UpdatedAt = now,
            IsDeleted = false,
            Receivers = new List<NotificationReceiver>()
        };

        foreach (var (role, userId) in recipients)
        {
            notification.Receivers.Add(new NotificationReceiver
            {
                NotificationId = notification.Id,
                UserType = role,
                UserId = userId,
                UserFullName = await ResolveNameAsync(role, userId),
                IsRead = false,
                ReadAt = null,
                IsArchived = false
            });
        }

        context.Notifications.Add(notification);
        await context.SaveChangesAsync();

        // إعادة ملء المعرّف الأجنبي في صفوف المستلمين (موجود تلقائيًا عبر EF)
        await BroadcastAsync(notification, recipients);

        return notification;
    }

    // ─────────────────────────────────────────────────────────────
    // وصول: تفاصيل / وارد / مرسل / قراءة / أرشفة / بحث
    // ─────────────────────────────────────────────────────────────

    public async Task<NotificationDetailDto?> GetDetailAsync(int id)
    {
        var notification = await repository.GetByIdWithReceiversAsync(id);
        return notification is null ? null : mapper.Map<NotificationDetailDto>(notification);
    }

    public async Task<List<NotificationInboxItemDto>> GetInboxAsync(
        string role, int userId, NotificationFilter? filter)
    {
        var receivers = await context.NotificationReceivers
            .Where(r => r.UserType == role && r.UserId == userId)
            .ToListAsync();

        if (filter?.Archived is not null)
        {
            receivers = receivers.Where(r => r.IsArchived == filter.Archived).ToList();
        }

        var notificationIds = receivers.Select(r => r.NotificationId).Distinct().ToList();
        var notifications = await repository.GetInboxNotificationsAsync(notificationIds, filter);

        var receiverByNotif = receivers.ToLookup(r => r.NotificationId);

        return notifications.Select(n =>
        {
            var rec = receiverByNotif[n.Id].FirstOrDefault();
            return new NotificationInboxItemDto
            {
                Id = n.Id,
                Title = n.Title,
                Message = n.Message,
                Category = n.Category,
                Priority = n.Priority,
                AttachmentPath = n.AttachmentPath,
                Status = n.Status,
                RecipientLabel = n.RecipientLabel,
                SenderRole = n.SenderType,
                SenderId = n.SenderId,
                SenderName = n.SenderName,
                CreatedAt = n.CreatedAt,
                UpdatedAt = n.UpdatedAt,
                IsDeleted = n.IsDeleted,
                ReceiverId = rec?.Id ?? 0,
                IsRead = rec?.IsRead ?? false,
                ReadAt = rec?.ReadAt,
                IsArchived = rec?.IsArchived ?? false
            };
        }).ToList();
    }

    public async Task<List<NotificationSentItemDto>> GetSentAsync(
        string role, int userId, NotificationFilter? filter)
    {
        var notifications = await repository.GetSentNotificationsAsync(role, userId, filter);
        if (notifications.Count == 0)
        {
            return new List<NotificationSentItemDto>();
        }

        var ids = notifications.Select(n => n.Id).ToList();
        var stats = await context.NotificationReceivers
            .Where(r => ids.Contains(r.NotificationId))
            .GroupBy(r => r.NotificationId)
            .Select(g => new
            {
                NotificationId = g.Key,
                Count = g.Count(),
                ReadCount = g.Count(r => r.IsRead)
            })
            .ToListAsync();

        var statsByNotif = stats.ToDictionary(s => s.NotificationId);

        return notifications.Select(n =>
        {
            var stat = statsByNotif.GetValueOrDefault(n.Id);
            return new NotificationSentItemDto
            {
                Id = n.Id,
                Title = n.Title,
                Message = n.Message,
                Category = n.Category,
                Priority = n.Priority,
                AttachmentPath = n.AttachmentPath,
                Status = n.Status,
                RecipientLabel = n.RecipientLabel,
                SenderRole = n.SenderType,
                SenderId = n.SenderId,
                SenderName = n.SenderName,
                CreatedAt = n.CreatedAt,
                UpdatedAt = n.UpdatedAt,
                IsDeleted = n.IsDeleted,
                RecipientsCount = stat?.Count ?? 0,
                ReadCount = stat?.ReadCount ?? 0
            };
        }).ToList();
    }

    public async Task<int> GetUnreadCountAsync(string role, int userId)
        => await context.NotificationReceivers
            .CountAsync(r => r.UserType == role && r.UserId == userId && !r.IsRead);

    public async Task MarkReadAsync(int notificationId, string role, int userId)
    {
        var receiver = await FindReceiverAsync(notificationId, role, userId);
        receiver.IsRead = true;
        receiver.ReadAt ??= DateTime.UtcNow;
        await context.SaveChangesAsync();

        await PushUnreadCountAsync(role, userId);
    }

    public async Task SetArchivedAsync(int notificationId, string role, int userId, bool archived)
    {
        var receiver = await FindReceiverAsync(notificationId, role, userId);
        receiver.IsArchived = archived;
        await context.SaveChangesAsync();
    }

    public async Task<List<RecipientDto>> GetRecipientsAsync(int notificationId)
    {
        var receivers = await repository.GetReceiversForNotificationAsync(notificationId);
        return mapper.Map<List<RecipientDto>>(receivers);
    }

    public async Task<NotificationSearchResultDto> SearchAsync(
        string role, int userId, NotificationFilter? filter)
    {
        var inboxIds = await context.NotificationReceivers
            .Where(r => r.UserType == role && r.UserId == userId)
            .Select(r => r.NotificationId)
            .ToListAsync();

        var sentIds = await context.Notifications
            .Where(n => n.SenderType == role && n.SenderId == userId && !n.IsDeleted)
            .Select(n => n.Id)
            .ToListAsync();

        var ids = inboxIds.Union(sentIds).ToList();
        var notifications = await repository.GetInboxNotificationsAsync(ids, filter);

        var items = new List<NotificationDetailDto>();
        foreach (var n in notifications)
        {
            items.Add(mapper.Map<NotificationDetailDto>(n));
        }

        return new NotificationSearchResultDto
        {
            Items = items,
            Total = items.Count
        };
    }

    // ─────────────────────────────────────────────────────────────
    // الحذف الناعم (يرثها IService.DeleteAsync)
    // ─────────────────────────────────────────────────────────────

    public override async Task DeleteAsync(int id)
    {
        var notification = await context.Notifications.FirstOrDefaultAsync(n => n.Id == id)
            ?? throw new NotFoundException($"لا يوجد إشعار بالمعرّف {id}.");

        notification.IsDeleted = true;
        notification.UpdatedAt = DateTime.UtcNow;
        await context.SaveChangesAsync();
    }

    // ─────────────────────────────────────────────────────────────
    // حلّ المستلمين والتحقق من الصلاحيات
    // ─────────────────────────────────────────────────────────────

    private void Validate(CreateNotificationRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Title))
        {
            throw new BadRequestException("عنوان الإشعار مطلوب.");
        }

        if (string.IsNullOrWhiteSpace(request.Message))
        {
            throw new BadRequestException("نصّ الإشعار مطلوب.");
        }

        if (request.SenderId <= 0 || string.IsNullOrWhiteSpace(request.SenderRole))
        {
            throw new BadRequestException("هوية المرسل غير صالحة.");
        }

        if (request.Recipients is null || request.Recipients.Count == 0 ||
            request.Recipients.All(t => t.Role is null && IsEmpty(t.GroupIds) && IsEmpty(t.Users)))
        {
            throw new BadRequestException("يجب تحديد مستلم واحد على الأقل.");
        }
    }

    private static bool IsEmpty<T>(IEnumerable<T>? items) => items is null || !items.Any();

    private async Task<List<(string Role, int UserId)>> ResolveRecipientsAsync(
        CreateNotificationRequest request)
    {
        var teacherGroups = new HashSet<int>();
        if (request.SenderRole == "teacher")
        {
            teacherGroups = (await context.Groups
                    .Where(g => g.TeacherId == request.SenderId)
                    .Select(g => g.Id)
                    .ToListAsync())
                .ToHashSet();
        }

        var result = new HashSet<(string Role, int UserId)>();
        var labelParts = new List<string>();

        foreach (var target in request.Recipients)
        {
            if (!string.IsNullOrWhiteSpace(target.Role))
            {
                var role = target.Role.Trim().ToLowerInvariant();
                switch (role)
                {
                    case "all":
                        if (request.SenderRole == "teacher")
                        {
                            throw new BadRequestException(
                                "لا يمكن للأستاذ إرسال إشعار إلى جميع المستخدمين.");
                        }

                        await AddAllAsync(result);
                        break;

                    case "teachers":
                        await AddRoleUsersAsync(result, "teacher");
                        break;

                    case "students":
                        if (request.SenderRole == "teacher")
                        {
                            throw new BadRequestException(
                                "لا يمكن للأستاذ إرسال إشعار إلى جميع الطلبة، بل فقط إلى أفواجه.");
                        }

                        await AddRoleUsersAsync(result, "student");
                        break;

                    case "admins":
                        await AddRoleUsersAsync(result, "admin");
                        break;

                    case "parents":
                        await AddRoleUsersAsync(result, "parent");
                        break;

                    default:
                        throw new BadRequestException($"نوع المستلم غير معروف: '{role}'.");
                }
            }

            if (!IsEmpty(target.GroupIds))
            {
                foreach (var groupId in target.GroupIds!.Distinct())
                {
                    if (request.SenderRole == "teacher" && !teacherGroups.Contains(groupId))
                    {
                        throw new BadRequestException(
                            "لا يمكن للأستاذ إرسال إشعار إلى فوج لا يدرّسه.");
                    }

                    var groupStudents = await context.Students
                        .Where(s => s.GroupId == groupId)
                        .Select(s => new { s.Id, s.FullName })
                        .ToListAsync();

                    foreach (var s in groupStudents)
                    {
                        result.Add(("student", s.Id));
                    }
                }
            }

            if (!IsEmpty(target.Users))
            {
                foreach (var user in target.Users!)
                {
                    var role = user.Role.Trim().ToLowerInvariant();
                    if (role == "student" && request.SenderRole == "teacher")
                    {
                        var studentGroupId = await context.Students
                            .Where(s => s.Id == user.UserId)
                            .Select(s => s.GroupId)
                            .FirstOrDefaultAsync();

                        if (studentGroupId is null || !teacherGroups.Contains(studentGroupId.Value))
                        {
                            throw new BadRequestException(
                                $"لا يمكن للأستاذ إرسال إشعار إلى الطالب رقم {user.UserId} خارج أفواجه.");
                        }
                    }

                    result.Add((role, user.UserId));
                }
            }
        }

        if (result.Count == 0)
        {
            throw new BadRequestException("لم يُحلّ أي مستلم صالح من الأهداف المحددة.");
        }

        return result.ToList();
    }

    private async Task AddAllAsync(HashSet<(string Role, int UserId)> set)
    {
        await AddRoleUsersAsync(set, "admin");
        await AddRoleUsersAsync(set, "teacher");
        await AddRoleUsersAsync(set, "student");
    }

    private async Task AddRoleUsersAsync(HashSet<(string Role, int UserId)> set, string role)
    {
        switch (role)
        {
            case "admin":
                var adminIds = await context.Admins.Select(a => a.Id).ToListAsync();
                foreach (var id in adminIds) set.Add(("admin", id));
                break;
            case "teacher":
                var teacherIds = await context.Teachers
                    .Where(t => t.IsActive)
                    .Select(t => t.Id).ToListAsync();
                foreach (var id in teacherIds) set.Add(("teacher", id));
                break;
            case "student":
                var studentIds = await context.Students.Select(s => s.Id).ToListAsync();
                foreach (var id in studentIds) set.Add(("student", id));
                break;
            case "parent":
                var parentIds = await context.Parents.Select(p => p.Id).ToListAsync();
                foreach (var id in parentIds) set.Add(("parent", id));
                break;
        }
    }

    private async Task<string> ResolveNameAsync(string role, int userId)
    {
        switch (role)
        {
            case "admin":
                return await context.Admins
                    .Where(u => u.Id == userId).Select(u => u.FullName).FirstOrDefaultAsync() ?? string.Empty;
            case "teacher":
                return await context.Teachers
                    .Where(u => u.Id == userId).Select(u => u.FullName).FirstOrDefaultAsync() ?? string.Empty;
            case "student":
                return await context.Students
                    .Where(u => u.Id == userId).Select(u => u.FullName).FirstOrDefaultAsync() ?? string.Empty;
            case "parent":
                return await context.Parents
                    .Where(u => u.Id == userId).Select(u => u.FullName).FirstOrDefaultAsync() ?? string.Empty;
            default:
                return string.Empty;
        }
    }

    private static string BuildRecipientLabel(CreateNotificationRequest request)
    {
        var labels = new List<string>();

        foreach (var target in request.Recipients)
        {
            if (!string.IsNullOrWhiteSpace(target.Role))
            {
                switch (target.Role.Trim().ToLowerInvariant())
                {
                    case "all": labels.Add("جميع المستخدمين"); break;
                    case "teachers": labels.Add("الأساتذة"); break;
                    case "students": labels.Add("الطلبة"); break;
                    case "admins": labels.Add("الإداريون"); break;
                    case "parents": labels.Add("أولياء الأمور"); break;
                }
            }

            if (!IsEmpty(target.GroupIds))
            {
                labels.Add($"{target.GroupIds!.Count} أفواج");
            }

            if (!IsEmpty(target.Users))
            {
                labels.Add($"{target.Users!.Count} أفراد");
            }
        }

        return labels.Count == 0 ? "بدون مستلمين" : string.Join(" + ", labels);
    }

    private async Task<NotificationReceiver> FindReceiverAsync(
        int notificationId, string role, int userId)
    {
        var receiver = await context.NotificationReceivers
            .FirstOrDefaultAsync(r =>
                r.NotificationId == notificationId &&
                r.UserType == role &&
                r.UserId == userId);

        return receiver
            ?? throw new NotFoundException("هذا الإشعار غير موجّه إليك أو غير موجود.");
    }

    // ─────────────────────────────────────────────────────────────
    // البث الفوري عبر SignalR
    // ─────────────────────────────────────────────────────────────

    private async Task BroadcastAsync(
        AppNotification notification,
        List<(string Role, int UserId)> recipients)
    {
        var item = mapper.Map<NotificationInboxItemDto>(notification);
        item.IsRead = false;
        item.ReadAt = null;
        item.IsArchived = false;

        foreach (var (role, userId) in recipients.Distinct())
        {
            var unread = await GetUnreadCountAsync(role, userId);
            await hub.Clients
                .Group(NotificationHub.UserGroup(role, userId))
                .NotificationReceived(item);
            await hub.Clients
                .Group(NotificationHub.UserGroup(role, userId))
                .UnreadCountChanged(unread);
        }
    }

    private async Task PushUnreadCountAsync(string role, int userId)
    {
        var unread = await GetUnreadCountAsync(role, userId);
        await hub.Clients
            .Group(NotificationHub.UserGroup(role, userId))
            .UnreadCountChanged(unread);
    }

    public async Task<List<AdminUserDto>> GetAdminUsersAsync()
        => await context.Admins
            .Select(a => new AdminUserDto { Id = a.Id, Name = a.FullName })
            .ToListAsync();
}
