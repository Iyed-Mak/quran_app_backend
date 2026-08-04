using QuranSchool.Api.Models;

namespace QuranSchool.Api.Repositories.Interfaces;

public interface IExamResultRepository : IRepository<ExamResult>
{
    Task<List<ExamResult>> GetByExamAsync(int examId);
    Task<List<ExamResult>> GetByStudentAsync(int studentId);
}
