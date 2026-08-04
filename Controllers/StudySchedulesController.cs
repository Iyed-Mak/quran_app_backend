using Microsoft.AspNetCore.Mvc;
using QuranSchool.Api.Models;
using QuranSchool.Api.Services.Interfaces;

namespace QuranSchool.Api.Controllers;

[Route("api/[controller]")]
public class StudySchedulesController : BaseCrudController<StudySchedule, IStudyScheduleService>
{
    public StudySchedulesController(IStudyScheduleService service) : base(service)
    {
    }

    [HttpGet("by-group/{groupId:int}")]
    public async Task<IActionResult> GetByGroup(int groupId)
        => Ok(await _service.GetByGroupAsync(groupId));
}
