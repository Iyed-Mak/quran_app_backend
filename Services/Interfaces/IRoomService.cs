using QuranSchool.Api.Models;

namespace QuranSchool.Api.Services.Interfaces;

public interface IRoomService : IService<Room>
{
    Task<List<Room>> GetByCampusAsync(int campusId);
}
