using Microsoft.AspNetCore.Mvc;
using QuranSchool.Api.Models;
using QuranSchool.Api.Services.Interfaces;

namespace QuranSchool.Api.Controllers;

[Route("api/[controller]")]
public class SemestersController : BaseCrudController<Semester, ISemesterService>
{
    public SemestersController(ISemesterService service) : base(service)
    {
    }

    [HttpGet("by-academic-year/{academicYearId:int}")]
    public async Task<IActionResult> GetByAcademicYear(int academicYearId)
        => Ok(await _service.GetByAcademicYearAsync(academicYearId));
}
