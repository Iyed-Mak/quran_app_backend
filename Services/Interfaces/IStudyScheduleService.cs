using QuranSchool.Api.Models;

namespace QuranSchool.Api.Services.Interfaces;

public interface IStudyScheduleService : IService<StudySchedule>
{
    Task<List<StudySchedule>> GetByGroupAsync(int groupId);
}
