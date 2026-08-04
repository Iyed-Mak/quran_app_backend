using Microsoft.EntityFrameworkCore;
using QuranSchool.Api.Data;
using QuranSchool.Api.Models;
using QuranSchool.Api.Repositories.Interfaces;

namespace QuranSchool.Api.Repositories.Implementations;

public class SemesterRepository(AppDbContext context) : Repository<Semester>(context), ISemesterRepository
{
    public async Task<List<Semester>> GetByAcademicYearAsync(int academicYearId)
        => await _context.Semesters
            .Where(s => s.AcademicYearId == academicYearId)
            .OrderBy(s => s.OrderIndex)
            .AsNoTracking()
            .ToListAsync();
}
