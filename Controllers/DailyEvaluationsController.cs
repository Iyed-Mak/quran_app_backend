using Microsoft.AspNetCore.Mvc;
using QuranSchool.Api.Models;
using QuranSchool.Api.Services.Interfaces;

namespace QuranSchool.Api.Controllers;

[Route("api/[controller]")]
public class DailyEvaluationsController : BaseCrudController<DailyEvaluation, IDailyEvaluationService>
{
    public DailyEvaluationsController(IDailyEvaluationService service) : base(service)
    {
    }

    [HttpGet("by-student/{studentId:int}")]
    public async Task<IActionResult> GetByStudent(int studentId)
        => Ok(await _service.GetByStudentAsync(studentId));

    [HttpGet("by-student/{studentId:int}/date/{date}")]
    public async Task<IActionResult> GetByStudentAndDate(int studentId, DateOnly date)
    {
        var evaluation = await _service.GetByStudentAndDateAsync(studentId, date);
        return evaluation is null ? NotFound() : Ok(evaluation);
    }
}
