using QuranSchool.Api.Models;

namespace QuranSchool.Api.Services.Interfaces;

public interface IExamService : IService<Exam>
{
    Task<List<Exam>> GetByExamPlanAsync(int examPlanId);
    Task<List<Exam>> GetByGroupAsync(int groupId);
}
