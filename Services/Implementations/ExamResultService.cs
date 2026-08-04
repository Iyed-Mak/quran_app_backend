using QuranSchool.Api.Models;
using QuranSchool.Api.Repositories.Interfaces;
using QuranSchool.Api.Services.Interfaces;

namespace QuranSchool.Api.Services.Implementations;

public class ExamResultService(IExamResultRepository repository) : Service<ExamResult>(repository), IExamResultService
{
    public async Task<List<ExamResult>> GetByExamAsync(int examId)
        => await repository.GetByExamAsync(examId);

    public async Task<List<ExamResult>> GetByStudentAsync(int studentId)
        => await repository.GetByStudentAsync(studentId);
}
