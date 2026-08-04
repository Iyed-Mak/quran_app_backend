using Microsoft.AspNetCore.Mvc;
using QuranSchool.Api.Models;
using QuranSchool.Api.Services.Interfaces;

namespace QuranSchool.Api.Controllers;

[Route("api/[controller]")]
public class TeacherAttendancesController : BaseCrudController<TeacherAttendance, ITeacherAttendanceService>
{
    public TeacherAttendancesController(ITeacherAttendanceService service) : base(service)
    {
    }

    [HttpGet("by-teacher/{teacherId:int}")]
    public async Task<IActionResult> GetByTeacher(int teacherId)
        => Ok(await _service.GetByTeacherAsync(teacherId));

    [HttpGet("by-group/{groupId:int}")]
    public async Task<IActionResult> GetByGroup(int groupId)
        => Ok(await _service.GetByGroupAsync(groupId));
}
