using QuranSchool.Api.Models;
using QuranSchool.Api.Repositories.Interfaces;
using QuranSchool.Api.Services.Interfaces;

namespace QuranSchool.Api.Services.Implementations;

public class ExamPlanService(IExamPlanRepository repository) : Service<ExamPlan>(repository), IExamPlanService
{
    public async Task<List<ExamPlan>> GetBySemesterAsync(int semesterId)
        => await repository.GetBySemesterAsync(semesterId);
}
