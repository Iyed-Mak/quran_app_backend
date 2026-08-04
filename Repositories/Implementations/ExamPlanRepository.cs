using Microsoft.EntityFrameworkCore;
using QuranSchool.Api.Data;
using QuranSchool.Api.Models;
using QuranSchool.Api.Repositories.Interfaces;

namespace QuranSchool.Api.Repositories.Implementations;

public class ExamPlanRepository(AppDbContext context) : Repository<ExamPlan>(context), IExamPlanRepository
{
    public async Task<List<ExamPlan>> GetBySemesterAsync(int semesterId)
        => await _context.ExamPlans
            .Where(p => p.SemesterId == semesterId)
            .AsNoTracking()
            .ToListAsync();
}
