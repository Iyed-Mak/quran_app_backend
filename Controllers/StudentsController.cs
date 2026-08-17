using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuranSchool.Api.DTOs.Student;
using QuranSchool.Api.Models;
using QuranSchool.Api.Services.Interfaces;

namespace QuranSchool.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class StudentsController(IStudentService service) : ControllerBase
{
    private readonly IStudentService _service = service;

    [HttpGet]
    public async Task<IActionResult> GetAll()
        => Ok(await _service.GetAllAsync());

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var student = await _service.GetByIdAsync(id);
        return student is null ? NotFound() : Ok(student);
    }

    [HttpGet("by-username/{username}")]
    public async Task<IActionResult> GetByUsername(string username)
    {
        var student = await _service.GetByUsernameAsync(username);
        return student is null ? NotFound() : Ok(student);
    }

    [HttpGet("by-group/{groupId:int}")]
    public async Task<IActionResult> GetByGroup(int groupId)
        => Ok(await _service.GetByGroupAsync(groupId));

    [HttpGet("by-parent/{parentId:int}")]
    public async Task<IActionResult> GetByParent(int parentId)
        => Ok(await _service.GetByParentAsync(parentId));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateStudentRequest request)
    {
        var created = await _service.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateStudentRequest request)
    {
        var updated = await _service.UpdateAsync(id, request);
        return Ok(updated);
    }

    [HttpPatch("{id:int}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateStudentStatusRequest request)
    {
        var updated = await _service.UpdateStatusAsync(id, request);
        return Ok(updated);
    }

    [HttpPost("{id:int}/reset-password")]
    public async Task<IActionResult> ResetPassword(int id, [FromBody] ResetPasswordRequest? request)
    {
        var password = await _service.ResetPasswordAsync(id, request?.NewPassword);
        return Ok(new { password });
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.DeleteAsync(id);
        return NoContent();
    }
}
