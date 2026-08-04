using QuranSchool.Api.Models;

namespace QuranSchool.Api.Repositories.Interfaces;

public interface ICampusRepository : IRepository<Campus>
{
    Task<List<Campus>> GetWithRoomsAsync();
}
