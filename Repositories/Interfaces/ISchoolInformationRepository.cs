using QuranSchool.Api.Models;

namespace QuranSchool.Api.Repositories.Interfaces;

public interface ISchoolInformationRepository : IRepository<SchoolInformation>
{
    Task<SchoolInformation?> GetWithDetailsAsync();
    Task AddWorkingHoursAsync(SchoolWorkingHours entity);
    void UpdateEntity<T>(T entity) where T : class;
    Task<SchoolWorkingPeriod?> GetPeriodByIdAsync(int periodId);
    Task DeletePeriodAsync(SchoolWorkingPeriod period);
    Task AddRuleAsync(SchoolRule entity);
    Task<SchoolRule?> GetRuleByIdAsync(int ruleId);
    Task DeleteRuleAsync(SchoolRule rule);
}
