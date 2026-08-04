using QuranSchool.Api.Exceptions;
using QuranSchool.Api.Models;
using QuranSchool.Api.Repositories.Interfaces;
using QuranSchool.Api.Services.Interfaces;

namespace QuranSchool.Api.Services.Implementations;

public class RoomService(
    IRoomRepository repository,
    IStudyScheduleRepository schedules,
    IExamRepository exams
) : Service<Room>(repository), IRoomService
{
    public async Task<List<Room>> GetByCampusAsync(int campusId)
        => await repository.GetByCampusAsync(campusId);

    public override async Task<Room> CreateAsync(Room entity)
    {
        await _EnsureUniqueNameAsync(entity);
        return await base.CreateAsync(entity);
    }

    public override async Task UpdateAsync(Room entity)
    {
        await _EnsureUniqueNameAsync(entity);
        await base.UpdateAsync(entity);
    }

    private async Task _EnsureUniqueNameAsync(Room entity)
    {
        entity.Name = entity.Name.Trim();
        if (string.IsNullOrWhiteSpace(entity.Name))
        {
            throw new BadRequestException("اسم الحجرة مطلوب.");
        }

        if (await repository.ExistsByNameAsync(entity.Name, entity.Id))
        {
            throw new BadRequestException(
                $"يوجد بالفعل حجرة باسم «{entity.Name}». يجب أن تكون أسماء الحجرات فريدة."
            );
        }
    }

    public override async Task DeleteAsync(int id)
    {
        if (await exams.ExistsForRoomAsync(id))
        {
            throw new BadRequestException(
                "لا يمكن حذف هذه الحجرة لأنها مرتبطة بامتحانات مجدولة. ألغِ الامتحانات أولاً."
            );
        }

        await schedules.DeleteByRoomAsync(id);

        var entity = await _repository.GetByIdAsync(id);
        if (entity is null)
        {
            return;
        }

        _repository.Delete(entity);
        await _repository.SaveChangesAsync();
    }
}
