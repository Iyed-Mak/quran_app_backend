using QuranSchool.Api.Models;

namespace QuranSchool.Api.Repositories.Interfaces;

public interface ITeacherRepository : IRepository<Teacher>
{
    Task<Teacher?> GetByUsernameAsync(string username);
}
