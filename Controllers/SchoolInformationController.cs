using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuranSchool.Api.DTOs.SchoolInformation;
using QuranSchool.Api.Services.Interfaces;

namespace QuranSchool.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SchoolInformationController(ISchoolInformationService service) : ControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Get()
    {
        var result = await service.GetAsync();
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> CreateOrUpdate([FromBody] UpdateSchoolInfoRequest request)
    {
        var result = await service.CreateOrUpdateAsync(request);
        return Ok(result);
    }

    [HttpPut("working-hours")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> UpdateWorkingHours([FromBody] UpdateWorkingHoursRequest request)
    {
        var result = await service.UpdateWorkingHoursAsync(request);
        return Ok(result);
    }

    [HttpDelete("working-periods/{id:int}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> DeleteWorkingPeriod(int id)
    {
        await service.DeleteWorkingPeriodAsync(id);
        return NoContent();
    }

    [HttpPost("rules")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> CreateRule([FromBody] CreateSchoolRuleRequest request)
    {
        var result = await service.CreateRuleAsync(request);
        return CreatedAtAction(nameof(Get), result);
    }

    [HttpPut("rules/{id:int}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> UpdateRule(int id, [FromBody] UpdateSchoolRuleRequest request)
    {
        var result = await service.UpdateRuleAsync(id, request);
        return Ok(result);
    }

    [HttpDelete("rules/{id:int}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> DeleteRule(int id)
    {
        await service.DeleteRuleAsync(id);
        return NoContent();
    }

    [HttpPut("rules/reorder")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> ReorderRules([FromBody] ReorderRulesRequest request)
    {
        await service.ReorderRulesAsync(request);
        return NoContent();
    }
}
