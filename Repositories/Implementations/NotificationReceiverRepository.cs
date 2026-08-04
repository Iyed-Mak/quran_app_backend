using Microsoft.EntityFrameworkCore;
using QuranSchool.Api.Data;
using QuranSchool.Api.Models;
using QuranSchool.Api.Repositories.Interfaces;

namespace QuranSchool.Api.Repositories.Implementations;

public class NotificationReceiverRepository(AppDbContext context) : Repository<NotificationReceiver>(context), INotificationReceiverRepository
{
    public async Task<List<NotificationReceiver>> GetByUserAsync(string userType, int userId)
        => await _context.NotificationReceivers
            .Where(r => r.UserType == userType && r.UserId == userId)
            .OrderByDescending(r => r.Id)
            .AsNoTracking()
            .ToListAsync();

    public async Task<int> GetUnreadCountAsync(string userType, int userId)
        => await _context.NotificationReceivers
            .CountAsync(r => r.UserType == userType && r.UserId == userId && !r.IsRead);
}
