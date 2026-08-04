using QuranSchool.Api.Models;
using QuranSchool.Api.Repositories.Interfaces;
using QuranSchool.Api.Services.Interfaces;

namespace QuranSchool.Api.Services.Implementations;

public class DailyEvaluationService(IDailyEvaluationRepository repository) : Service<DailyEvaluation>(repository), IDailyEvaluationService
{
    public async Task<List<DailyEvaluation>> GetByStudentAsync(int studentId)
        => await repository.GetByStudentAsync(studentId);

    public async Task<DailyEvaluation?> GetByStudentAndDateAsync(int studentId, DateOnly date)
        => await repository.GetByStudentAndDateAsync(studentId, date);
}
