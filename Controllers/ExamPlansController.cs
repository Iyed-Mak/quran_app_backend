using Microsoft.AspNetCore.Mvc;
using QuranSchool.Api.Models;
using QuranSchool.Api.Services.Interfaces;

namespace QuranSchool.Api.Controllers;

[Route("api/[controller]")]
public class ExamPlansController : BaseCrudController<ExamPlan, IExamPlanService>
{
    public ExamPlansController(IExamPlanService service) : base(service)
    {
    }

    [HttpGet("by-semester/{semesterId:int}")]
    public async Task<IActionResult> GetBySemester(int semesterId)
        => Ok(await _service.GetBySemesterAsync(semesterId));
}
