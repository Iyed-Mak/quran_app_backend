using QuranSchool.Api.DTOs.SchoolInformation;
using QuranSchool.Api.Models;

namespace QuranSchool.Api.Services.Interfaces;

public interface ISchoolInformationService
{
    Task<SchoolInfoResponse?> GetAsync();
    Task<SchoolInfoResponse> CreateOrUpdateAsync(UpdateSchoolInfoRequest request);
    Task<SchoolWorkingHoursDto> UpdateWorkingHoursAsync(UpdateWorkingHoursRequest request);
    Task DeleteWorkingPeriodAsync(int periodId);
    Task<SchoolRuleDto> CreateRuleAsync(CreateSchoolRuleRequest request);
    Task<SchoolRuleDto> UpdateRuleAsync(int id, UpdateSchoolRuleRequest request);
    Task DeleteRuleAsync(int id);
    Task ReorderRulesAsync(ReorderRulesRequest request);
}
