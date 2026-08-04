using Microsoft.EntityFrameworkCore;
using QuranSchool.Api.Data;
using QuranSchool.Api.Models;
using QuranSchool.Api.Repositories.Interfaces;

namespace QuranSchool.Api.Repositories.Implementations;

public class RoomRepository(AppDbContext context) : Repository<Room>(context), IRoomRepository
{
    public async Task<List<Room>> GetByCampusAsync(int campusId)
        => await _context.Rooms
            .Where(r => r.CampusId == campusId)
            .AsNoTracking()
            .ToListAsync();

    public async Task DeleteByCampusAsync(int campusId)
        => await _context.Rooms
            .Where(r => r.CampusId == campusId)
            .ExecuteDeleteAsync();
}
