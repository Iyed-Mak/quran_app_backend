using QuranSchool.Api.Models;

namespace QuranSchool.Api.Repositories.Interfaces;

public interface IParentRepository : IRepository<Parent>
{
    Task<Parent?> GetByUsernameAsync(string username);
}
