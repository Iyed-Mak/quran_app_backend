using QuranSchool.Api.Models;

namespace QuranSchool.Api.Services.Interfaces;

public interface IExamResultService : IService<ExamResult>
{
    Task<List<ExamResult>> GetByExamAsync(int examId);
    Task<List<ExamResult>> GetByStudentAsync(int studentId);
}
