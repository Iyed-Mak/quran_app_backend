using QuranSchool.Api.Models;

namespace QuranSchool.Api.Services.Interfaces;

public interface IDailyEvaluationService : IService<DailyEvaluation>
{
    Task<List<DailyEvaluation>> GetByStudentAsync(int studentId);
    Task<DailyEvaluation?> GetByStudentAndDateAsync(int studentId, DateOnly date);
}
