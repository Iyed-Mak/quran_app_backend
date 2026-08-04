using QuranSchool.Api.Models;

namespace QuranSchool.Api.Repositories.Interfaces;

public interface IHomeworkRepository : IRepository<Homework>
{
    Task<List<Homework>> GetByStudentAsync(int studentId);
    Task<List<Homework>> GetByTeacherAsync(int teacherId);
}
