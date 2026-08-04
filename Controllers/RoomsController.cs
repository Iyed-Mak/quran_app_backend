using Microsoft.AspNetCore.Mvc;
using QuranSchool.Api.Models;
using QuranSchool.Api.Services.Interfaces;

namespace QuranSchool.Api.Controllers;

[Route("api/[controller]")]
public class RoomsController : BaseCrudController<Room, IRoomService>
{
    public RoomsController(IRoomService service) : base(service)
    {
    }

    [HttpGet("by-campus/{campusId:int}")]
    public async Task<IActionResult> GetByCampus(int campusId)
        => Ok(await _service.GetByCampusAsync(campusId));
}
