using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuranSchool.Api.DTOs.Statistics;
using QuranSchool.Api.Services.Interfaces;

namespace QuranSchool.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/admin/statistics")]
public class StatisticsController(IStatisticsService service) : ControllerBase
{
    [HttpGet("overview")]
    public async Task<IActionResult> GetOverview()
        => Ok(await service.GetOverviewAsync());

    [HttpGet("students")]
    public async Task<IActionResult> GetStudents(
        [FromQuery] string? gender,
        [FromQuery] string? ageOperator,
        [FromQuery] int? ageValue,
        [FromQuery] string? regDateOperator,
        [FromQuery] DateOnly? regDate,
        [FromQuery] string? status,
        [FromQuery] int? groupId,
        [FromQuery] int? campusId)
    {
        Console.WriteLine($"━━━ STATISTICS FILTER ━━━");
        Console.WriteLine($"gender: {gender}");
        Console.WriteLine($"ageOperator: {ageOperator}");
        Console.WriteLine($"ageValue: {ageValue}");
        Console.WriteLine($"regDateOperator: {regDateOperator}");
        Console.WriteLine($"regDate: {regDate}");
        Console.WriteLine($"status: {status}");
        Console.WriteLine($"groupId: {groupId}");
        Console.WriteLine($"campusId: {campusId}");
        Console.WriteLine($"Request QueryString: {HttpContext.Request.QueryString}");
        Console.WriteLine($"All Query Params: {string.Join(", ", HttpContext.Request.Query.Select(kv => $"{kv.Key}={kv.Value}"))}");

        var result = await service.GetStudentStatisticsAsync(
            gender, ageOperator, ageValue,
            regDateOperator, regDate,
            status, groupId, campusId);

        Console.WriteLine($"Result: {result.TotalCount} students");
        return Ok(result);
    }

    [HttpGet("registrations")]
    public async Task<IActionResult> GetRegistrations(
        [FromQuery] string? period,
        [FromQuery] DateOnly? dateFrom,
        [FromQuery] DateOnly? dateTo)
        => Ok(await service.GetRegistrationStatisticsAsync(period, dateFrom, dateTo));

    [HttpGet("groups")]
    public async Task<IActionResult> GetGroups()
        => Ok(await service.GetGroupStatisticsAsync());

    [HttpGet("teachers")]
    public async Task<IActionResult> GetTeachers()
        => Ok(await service.GetTeacherStatisticsAsync());

    [HttpGet("campuses")]
    public async Task<IActionResult> GetCampuses()
        => Ok(await service.GetCampusStatisticsAsync());

    [HttpGet("rooms")]
    public async Task<IActionResult> GetRooms()
        => Ok(await service.GetRoomStatisticsAsync());

    [HttpGet("attendance")]
    public async Task<IActionResult> GetAttendance(
        [FromQuery] string? period,
        [FromQuery] DateOnly? dateFrom,
        [FromQuery] DateOnly? dateTo)
        => Ok(await service.GetAttendanceStatisticsAsync(period, dateFrom, dateTo));

    [HttpGet("academic")]
    public async Task<IActionResult> GetAcademic()
        => Ok(await service.GetAcademicStatisticsAsync());

    [HttpGet("exams")]
    public async Task<IActionResult> GetExams(
        [FromQuery] int? semesterId,
        [FromQuery] int? groupId,
        [FromQuery] string? gender)
        => Ok(await service.GetExamStatisticsAsync(semesterId, groupId, gender));
}
