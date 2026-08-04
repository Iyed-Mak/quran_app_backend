using QuranSchool.Api.Models;

namespace QuranSchool.Api.Repositories.Interfaces;

public interface IStudentRepository : IRepository<Student>
{
    Task<Student?> GetByUsernameAsync(string username);
    Task<List<Student>> GetByGroupAsync(int groupId);
    Task<List<Student>> GetByParentAsync(int parentId);
    Task<int> GetMaxStudentNumberAsync();
}
