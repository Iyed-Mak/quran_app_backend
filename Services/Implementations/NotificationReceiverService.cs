using QuranSchool.Api.Models;
using QuranSchool.Api.Repositories.Interfaces;
using QuranSchool.Api.Services.Interfaces;

namespace QuranSchool.Api.Services.Implementations;

public class NotificationReceiverService(INotificationReceiverRepository repository) : Service<NotificationReceiver>(repository), INotificationReceiverService
{
    public async Task<List<NotificationReceiver>> GetByUserAsync(string userType, int userId)
        => await repository.GetByUserAsync(userType, userId);

    public async Task<int> GetUnreadCountAsync(string userType, int userId)
        => await repository.GetUnreadCountAsync(userType, userId);
}
