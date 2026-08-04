using QuranSchool.Api.Models;

namespace QuranSchool.Api.Services.Interfaces;

public interface INotificationReceiverService : IService<NotificationReceiver>
{
    Task<List<NotificationReceiver>> GetByUserAsync(string userType, int userId);
    Task<int> GetUnreadCountAsync(string userType, int userId);
}
