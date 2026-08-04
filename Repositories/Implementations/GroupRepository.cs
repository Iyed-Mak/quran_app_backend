using Microsoft.EntityFrameworkCore;
using QuranSchool.Api.Data;
using QuranSchool.Api.Models;
using QuranSchool.Api.Repositories.Interfaces;

namespace QuranSchool.Api.Repositories.Implementations;

public class GroupRepository(AppDbContext context) : Repository<Group>(context), IGroupRepository
{
    public async Task<List<Group>> GetByAcademicYearAsync(int academicYearId)
        => await _context.Groups
            .Where(g => g.AcademicYearId == academicYearId)
            .AsNoTracking()
            .ToListAsync();

    public async Task<List<Group>> GetByTeacherAsync(int teacherId)
        => await _context.Groups
            .Where(g => g.TeacherId == teacherId)
            .AsNoTracking()
            .ToListAsync();
}
