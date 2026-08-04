using QuranSchool.Api.Models;

namespace QuranSchool.Api.Services.Interfaces;

public interface IHomeworkService : IService<Homework>
{
    Task<List<Homework>> GetByStudentAsync(int studentId);
    Task<List<Homework>> GetByTeacherAsync(int teacherId);
}
