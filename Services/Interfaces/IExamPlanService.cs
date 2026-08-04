using QuranSchool.Api.Models;

namespace QuranSchool.Api.Services.Interfaces;

public interface IExamPlanService : IService<ExamPlan>
{
    Task<List<ExamPlan>> GetBySemesterAsync(int semesterId);
}
