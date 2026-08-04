using Microsoft.EntityFrameworkCore;
using QuranSchool.Api.Data;
using QuranSchool.Api.Models;
using QuranSchool.Api.Repositories.Interfaces;

namespace QuranSchool.Api.Repositories.Implementations;

public class CampusRepository(AppDbContext context) : Repository<Campus>(context), ICampusRepository
{
    public async Task<List<Campus>> GetWithRoomsAsync()
        => await _context.Campuses.Include(c => c.Rooms).AsNoTracking().ToListAsync();
}
