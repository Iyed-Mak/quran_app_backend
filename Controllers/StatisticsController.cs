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
    private static DateOnly? ParseDate(string? s)
        => DateOnly.TryParse(s, out var d) ? d : null;

    [HttpGet("overview")]
    public async Task<IActionResult> GetOverview()
        => Ok(await service.GetOverviewAsync());

    [HttpGet("students")]
    public async Task<IActionResult> GetStudents(
        [FromQuery] string? gender,
        [FromQuery] string? ageOperator,
        [FromQuery] int? ageValue,
        [FromQuery] string? regDateOperator,
        [FromQuery] string? regDate,
        [FromQuery] string? status,
        [FromQuery] int? groupId,
        [FromQuery] int? campusId)
        => Ok(await service.GetStudentStatisticsAsync(
            gender, ageOperator, ageValue,
            regDateOperator, ParseDate(regDate),
            status, groupId, campusId));

    [HttpGet("registrations")]
    public async Task<IActionResult> GetRegistrations(
        [FromQuery] string? period,
        [FromQuery] string? dateFrom,
        [FromQuery] string? dateTo)
        => Ok(await service.GetRegistrationStatisticsAsync(period, ParseDate(dateFrom), ParseDate(dateTo)));

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
    public async Task<IActionResult> GetRooms(
        [FromQuery] string? weekday)
        => Ok(await service.GetRoomStatisticsAsync(weekday));

    [HttpGet("attendance")]
    public async Task<IActionResult> GetAttendance(
        [FromQuery] string? period,
        [FromQuery] string? dateFrom,
        [FromQuery] string? dateTo,
        [FromQuery] int? month,
        [FromQuery] int? year,
        [FromQuery] string? gender)
        => Ok(await service.GetAttendanceStatisticsAsync(period, ParseDate(dateFrom), ParseDate(dateTo), month, year, gender));

    [HttpGet("academic")]
    public async Task<IActionResult> GetAcademic(
        [FromQuery] string? dateFrom,
        [FromQuery] string? dateTo,
        [FromQuery] int? month,
        [FromQuery] int? year,
        [FromQuery] string? gender)
        => Ok(await service.GetAcademicStatisticsAsync(ParseDate(dateFrom), ParseDate(dateTo), month, year, gender));

    [HttpGet("exams")]
    public async Task<IActionResult> GetExams(
        [FromQuery] int? semesterId,
        [FromQuery] int? groupId,
        [FromQuery] string? gender)
        => Ok(await service.GetExamStatisticsAsync(semesterId, groupId, gender));
}
