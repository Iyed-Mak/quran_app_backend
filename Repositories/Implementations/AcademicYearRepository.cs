using Microsoft.EntityFrameworkCore;
using QuranSchool.Api.Data;
using QuranSchool.Api.Models;
using QuranSchool.Api.Repositories.Interfaces;

namespace QuranSchool.Api.Repositories.Implementations;

public class AcademicYearRepository(AppDbContext context) : Repository<AcademicYear>(context), IAcademicYearRepository
{
    public async Task<AcademicYear?> GetCurrentAsync()
        => await _context.AcademicYears.FirstOrDefaultAsync(y => y.IsCurrent);
}
