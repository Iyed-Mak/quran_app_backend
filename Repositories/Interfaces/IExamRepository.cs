using QuranSchool.Api.Models;

namespace QuranSchool.Api.Repositories.Interfaces;

public interface IExamRepository : IRepository<Exam>
{
    Task<List<Exam>> GetByExamPlanAsync(int examPlanId);
    Task<List<Exam>> GetByGroupAsync(int groupId);
    Task<bool> ExistsForCampusAsync(int campusId);
    Task<bool> ExistsForRoomAsync(int roomId);
}
