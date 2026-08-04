using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuranSchool.Api.DTOs.Parent;
using QuranSchool.Api.Models;
using QuranSchool.Api.Services.Interfaces;

namespace QuranSchool.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class ParentsController(IParentService service) : ControllerBase
{
    private readonly IParentService _service = service;

    [HttpGet]
    public async Task<IActionResult> GetAll()
        => Ok(await _service.GetAllAsync());

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var parent = await _service.GetByIdAsync(id);
        return parent is null ? NotFound() : Ok(parent);
    }

    [HttpGet("by-username/{username}")]
    public async Task<IActionResult> GetByUsername(string username)
    {
        var parent = await _service.GetByUsernameAsync(username);
        return parent is null ? NotFound() : Ok(parent);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateParentRequest request)
    {
        var created = await _service.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] Parent entity)
    {
        if (id != entity.Id)
        {
            return BadRequest("Route id does not match body id.");
        }

        await _service.UpdateAsync(entity);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.DeleteAsync(id);
        return NoContent();
    }
}
