using QuranSchool.Api.Models;
using QuranSchool.Api.Repositories.Interfaces;
using QuranSchool.Api.Services.Interfaces;

namespace QuranSchool.Api.Services.Implementations;

public class HomeworkService(IHomeworkRepository repository) : Service<Homework>(repository), IHomeworkService
{
    public async Task<List<Homework>> GetByStudentAsync(int studentId)
        => await repository.GetByStudentAsync(studentId);

    public async Task<List<Homework>> GetByTeacherAsync(int teacherId)
        => await repository.GetByTeacherAsync(teacherId);
}
