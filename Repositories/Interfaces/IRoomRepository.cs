using QuranSchool.Api.Models;

namespace QuranSchool.Api.Repositories.Interfaces;

public interface IRoomRepository : IRepository<Room>
{
    Task<List<Room>> GetByCampusAsync(int campusId);
    Task DeleteByCampusAsync(int campusId);
    Task<bool> ExistsByNameAsync(string name, int? excludeId = null);
}
