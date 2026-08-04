using Microsoft.AspNetCore.Mvc;
using QuranSchool.Api.Models;
using QuranSchool.Api.Services.Interfaces;

namespace QuranSchool.Api.Controllers;

[Route("api/[controller]")]
public class AcademicYearsController : BaseCrudController<AcademicYear, IAcademicYearService>
{
    public AcademicYearsController(IAcademicYearService service) : base(service)
    {
    }

    [HttpGet("current")]
    public async Task<IActionResult> GetCurrent()
    {
        var year = await _service.GetCurrentAsync();
        return year is null ? NotFound() : Ok(year);
    }
}
