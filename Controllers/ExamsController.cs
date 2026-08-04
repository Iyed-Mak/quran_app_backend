using Microsoft.AspNetCore.Mvc;
using QuranSchool.Api.Models;
using QuranSchool.Api.Services.Interfaces;

namespace QuranSchool.Api.Controllers;

[Route("api/[controller]")]
public class ExamsController : BaseCrudController<Exam, IExamService>
{
    public ExamsController(IExamService service) : base(service)
    {
    }

    [HttpGet("by-exam-plan/{examPlanId:int}")]
    public async Task<IActionResult> GetByExamPlan(int examPlanId)
        => Ok(await _service.GetByExamPlanAsync(examPlanId));

    [HttpGet("by-group/{groupId:int}")]
    public async Task<IActionResult> GetByGroup(int groupId)
        => Ok(await _service.GetByGroupAsync(groupId));
}
