using QuranSchool.Api.Exceptions;
using QuranSchool.Api.Models;
using QuranSchool.Api.Repositories.Interfaces;
using QuranSchool.Api.Services.Interfaces;

namespace QuranSchool.Api.Services.Implementations;

public class CampusService(
    ICampusRepository repository,
    IStudyScheduleRepository schedules,
    IRoomRepository rooms,
    IExamRepository exams
) : Service<Campus>(repository), ICampusService
{
    public async Task<List<Campus>> GetWithRoomsAsync()
        => await repository.GetWithRoomsAsync();

    public override async Task DeleteAsync(int id)
    {
        if (await exams.ExistsForCampusAsync(id))
        {
            throw new BadRequestException(
                "لا يمكن حذف هذا المقر لأنه مرتبط بامتحانات مجدولة. ألغِ الامتحانات أولاً."
            );
        }

        await schedules.DeleteByCampusAsync(id);
        await rooms.DeleteByCampusAsync(id);

        var entity = await _repository.GetByIdAsync(id);
        if (entity is null)
        {
            return;
        }

        _repository.Delete(entity);
        await _repository.SaveChangesAsync();
    }
}
