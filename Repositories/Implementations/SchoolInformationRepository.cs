using Microsoft.EntityFrameworkCore;
using QuranSchool.Api.Data;
using QuranSchool.Api.Models;
using QuranSchool.Api.Repositories.Interfaces;

namespace QuranSchool.Api.Repositories.Implementations;

public class SchoolInformationRepository(AppDbContext context)
    : Repository<SchoolInformation>(context), ISchoolInformationRepository
{
    public async Task<SchoolInformation?> GetWithDetailsAsync()
        => await _context.SchoolInformation
            .AsNoTracking()
            .Include(s => s.WorkingHours)
                .ThenInclude(w => w.Periods)
            .Include(s => s.Rules)
            .OrderBy(s => s.Id)
            .FirstOrDefaultAsync();

    public async Task AddWorkingHoursAsync(SchoolWorkingHours entity)
        => await _context.SchoolWorkingHours.AddAsync(entity);

    public void UpdateEntity<T>(T entity) where T : class
        => _context.Update(entity);

    public async Task<SchoolWorkingPeriod?> GetPeriodByIdAsync(int periodId)
        => await _context.SchoolWorkingPeriods.FirstOrDefaultAsync(p => p.Id == periodId);

    public async Task DeletePeriodAsync(SchoolWorkingPeriod period)
    {
        _context.SchoolWorkingPeriods.Remove(period);
        await _context.SaveChangesAsync();
    }

    public async Task AddRuleAsync(SchoolRule entity)
        => await _context.SchoolRules.AddAsync(entity);

    public async Task<SchoolRule?> GetRuleByIdAsync(int ruleId)
        => await _context.SchoolRules.FirstOrDefaultAsync(r => r.Id == ruleId);

    public async Task DeleteRuleAsync(SchoolRule rule)
    {
        _context.SchoolRules.Remove(rule);
        await _context.SaveChangesAsync();
    }
}
