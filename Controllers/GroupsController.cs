using Microsoft.AspNetCore.Mvc;
using QuranSchool.Api.Models;
using QuranSchool.Api.Services.Interfaces;

namespace QuranSchool.Api.Controllers;

[Route("api/[controller]")]
public class GroupsController : BaseCrudController<Group, IGroupService>
{
    public GroupsController(IGroupService service) : base(service)
    {
    }

    [HttpGet("by-academic-year/{academicYearId:int}")]
    public async Task<IActionResult> GetByAcademicYear(int academicYearId)
        => Ok(await _service.GetByAcademicYearAsync(academicYearId));

    [HttpGet("by-teacher/{teacherId:int}")]
    public async Task<IActionResult> GetByTeacher(int teacherId)
        => Ok(await _service.GetByTeacherAsync(teacherId));
}
