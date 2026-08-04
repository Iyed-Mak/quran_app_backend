using QuranSchool.Api.Models;

namespace QuranSchool.Api.Repositories.Interfaces;

public interface IExamPlanRepository : IRepository<ExamPlan>
{
    Task<List<ExamPlan>> GetBySemesterAsync(int semesterId);
}
