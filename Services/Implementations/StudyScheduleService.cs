using QuranSchool.Api.Models;
using QuranSchool.Api.Repositories.Interfaces;
using QuranSchool.Api.Services.Interfaces;

namespace QuranSchool.Api.Services.Implementations;

public class StudyScheduleService(IStudyScheduleRepository repository) : Service<StudySchedule>(repository), IStudyScheduleService
{
    public async Task<List<StudySchedule>> GetByGroupAsync(int groupId)
        => await repository.GetByGroupAsync(groupId);
}
