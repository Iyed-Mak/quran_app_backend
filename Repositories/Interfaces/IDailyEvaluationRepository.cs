using QuranSchool.Api.Models;

namespace QuranSchool.Api.Repositories.Interfaces;

public interface IDailyEvaluationRepository : IRepository<DailyEvaluation>
{
    Task<List<DailyEvaluation>> GetByStudentAsync(int studentId);
    Task<DailyEvaluation?> GetByStudentAndDateAsync(int studentId, DateOnly date);
}
