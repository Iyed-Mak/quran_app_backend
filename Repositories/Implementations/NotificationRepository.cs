using Microsoft.EntityFrameworkCore;
using QuranSchool.Api.Data;
using QuranSchool.Api.DTOs.Notification;
using QuranSchool.Api.Models;
using QuranSchool.Api.Repositories.Interfaces;

namespace QuranSchool.Api.Repositories.Implementations;

public class NotificationRepository(AppDbContext context) : Repository<AppNotification>(context), INotificationRepository
{
    public async Task<List<AppNotification>> GetBySenderAsync(string senderType, int senderId)
        => await _context.Notifications
            .Where(n => n.SenderType == senderType && n.SenderId == senderId && !n.IsDeleted)
            .OrderByDescending(n => n.CreatedAt)
            .AsNoTracking()
            .ToListAsync();

    public async Task<List<AppNotification>> GetLatestAsync(int count)
        => await _context.Notifications
            .Where(n => !n.IsDeleted)
            .OrderByDescending(n => n.CreatedAt)
            .Take(count)
            .AsNoTracking()
            .ToListAsync();

    public async Task<AppNotification?> GetByIdWithReceiversAsync(int id)
        => await _context.Notifications
            .Include(n => n.Receivers)
            .FirstOrDefaultAsync(n => n.Id == id && !n.IsDeleted);

    public async Task<List<AppNotification>> GetInboxNotificationsAsync(
        List<int> notificationIds,
        NotificationFilter? filter)
    {
        if (notificationIds.Count == 0)
        {
            return new List<AppNotification>();
        }

        var query = _context.Notifications
            .Where(n => !n.IsDeleted && notificationIds.Contains(n.Id));

        query = ApplyFilter(query, filter);

        return await query
            .OrderByDescending(n => n.CreatedAt)
            .Skip(Math.Max(0, ((filter?.Page ?? 1) - 1) * (filter?.PageSize ?? 100)))
            .Take(filter?.PageSize ?? 100)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<List<AppNotification>> GetSentNotificationsAsync(
        string senderType,
        int senderId,
        NotificationFilter? filter)
    {
        var query = _context.Notifications
            .Where(n => n.SenderType == senderType && n.SenderId == senderId && !n.IsDeleted);

        query = ApplyFilter(query, filter);

        return await query
            .OrderByDescending(n => n.CreatedAt)
            .Skip(Math.Max(0, ((filter?.Page ?? 1) - 1) * (filter?.PageSize ?? 100)))
            .Take(filter?.PageSize ?? 100)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<List<NotificationReceiver>> GetReceiversForNotificationAsync(int notificationId)
        => await _context.NotificationReceivers
            .Where(r => r.NotificationId == notificationId)
            .OrderBy(r => r.Id)
            .AsNoTracking()
            .ToListAsync();

    private static IQueryable<AppNotification> ApplyFilter(
        IQueryable<AppNotification> query,
        NotificationFilter? filter)
    {
        if (filter is null)
        {
            return query;
        }

        if (!string.IsNullOrWhiteSpace(filter.Keyword))
        {
            var keyword = filter.Keyword.Trim();
            query = query.Where(n =>
                n.Title.Contains(keyword) ||
                n.Message.Contains(keyword) ||
                n.SenderName.Contains(keyword));
        }

        if (!string.IsNullOrWhiteSpace(filter.Category))
        {
            query = query.Where(n => n.Category == filter.Category);
        }

        if (!string.IsNullOrWhiteSpace(filter.Priority))
        {
            query = query.Where(n => n.Priority == filter.Priority);
        }

        if (filter.From is not null)
        {
            query = query.Where(n => n.CreatedAt >= filter.From);
        }

        if (filter.To is not null)
        {
            query = query.Where(n => n.CreatedAt <= filter.To);
        }

        return query;
    }
}
