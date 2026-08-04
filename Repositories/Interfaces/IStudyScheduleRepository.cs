using QuranSchool.Api.Models;

namespace QuranSchool.Api.Repositories.Interfaces;

public interface IStudyScheduleRepository : IRepository<StudySchedule>
{
    Task<List<StudySchedule>> GetByGroupAsync(int groupId);
    Task DeleteByCampusAsync(int campusId);
    Task DeleteByRoomAsync(int roomId);
}
