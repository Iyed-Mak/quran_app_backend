using Microsoft.EntityFrameworkCore;
using QuranSchool.Api.Data;
using QuranSchool.Api.Models;
using QuranSchool.Api.Repositories.Interfaces;

namespace QuranSchool.Api.Repositories.Implementations;

public class DailyEvaluationRepository(AppDbContext context) : Repository<DailyEvaluation>(context), IDailyEvaluationRepository
{
    public async Task<List<DailyEvaluation>> GetByStudentAsync(int studentId)
        => await _context.DailyEvaluations
            .Where(e => e.StudentId == studentId)
            .AsNoTracking()
            .ToListAsync();

    public async Task<DailyEvaluation?> GetByStudentAndDateAsync(int studentId, DateOnly date)
        => await _context.DailyEvaluations
            .FirstOrDefaultAsync(e => e.StudentId == studentId && e.SessionDate == date);
}
