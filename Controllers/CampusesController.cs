using Microsoft.AspNetCore.Mvc;
using QuranSchool.Api.Models;
using QuranSchool.Api.Services.Interfaces;

namespace QuranSchool.Api.Controllers;

[Route("api/[controller]")]
public class CampusesController : BaseCrudController<Campus, ICampusService>
{
    public CampusesController(ICampusService service) : base(service)
    {
    }

    [HttpGet("with-rooms")]
    public async Task<IActionResult> GetWithRooms() => Ok(await _service.GetWithRoomsAsync());
}
