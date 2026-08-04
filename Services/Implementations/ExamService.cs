using QuranSchool.Api.Models;
using QuranSchool.Api.Repositories.Interfaces;
using QuranSchool.Api.Services.Interfaces;

namespace QuranSchool.Api.Services.Implementations;

public class ExamService(IExamRepository repository) : Service<Exam>(repository), IExamService
{
    public async Task<List<Exam>> GetByExamPlanAsync(int examPlanId)
        => await repository.GetByExamPlanAsync(examPlanId);

    public async Task<List<Exam>> GetByGroupAsync(int groupId)
        => await repository.GetByGroupAsync(groupId);
}
