using QuranSchool.Api.Models;

namespace QuranSchool.Api.Repositories.Interfaces;

public interface INotificationReceiverRepository : IRepository<NotificationReceiver>
{
    Task<List<NotificationReceiver>> GetByUserAsync(string userType, int userId);
    Task<int> GetUnreadCountAsync(string userType, int userId);
}
