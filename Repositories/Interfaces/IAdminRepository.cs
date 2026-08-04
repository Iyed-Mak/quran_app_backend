using QuranSchool.Api.Models;

namespace QuranSchool.Api.Repositories.Interfaces;

public interface IAdminRepository : IRepository<Admin>
{
    Task<Admin?> GetByUsernameAsync(string username);
}
