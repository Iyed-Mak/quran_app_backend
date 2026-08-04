using Microsoft.EntityFrameworkCore;
using QuranSchool.Api.Data;
using QuranSchool.Api.Models;
using QuranSchool.Api.Repositories.Interfaces;

namespace QuranSchool.Api.Repositories.Implementations;

public class StudyScheduleRepository(AppDbContext context) : Repository<StudySchedule>(context), IStudyScheduleRepository
{
    public async Task<List<StudySchedule>> GetByGroupAsync(int groupId)
        => await _context.StudySchedules
            .Where(s => s.GroupId == groupId)
            .OrderBy(s => s.Weekday)
            .AsNoTracking()
            .ToListAsync();

    public async Task DeleteByCampusAsync(int campusId)
        => await _context.StudySchedules
            .Where(s => s.CampusId == campusId)
            .ExecuteDeleteAsync();

    public async Task DeleteByRoomAsync(int roomId)
        => await _context.StudySchedules
            .Where(s => s.RoomId == roomId)
            .ExecuteDeleteAsync();
}
