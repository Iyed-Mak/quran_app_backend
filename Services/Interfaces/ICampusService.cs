using QuranSchool.Api.Models;

namespace QuranSchool.Api.Services.Interfaces;

public interface ICampusService : IService<Campus>
{
    Task<List<Campus>> GetWithRoomsAsync();
}
