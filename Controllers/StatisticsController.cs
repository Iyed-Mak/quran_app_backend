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
        [FromQuery] string? dateFilter,
        [FromQuery] DateOnly? dateFrom,
        [FromQuery] DateOnly? dateTo,
        [FromQuery] string? ageFilter,
        [FromQuery] int? ageMin,
        [FromQuery] int? ageMax,
        [FromQuery] string? status,
        [FromQuery] int? groupId,
        [FromQuery] int? campusId)
        => Ok(await service.GetStudentStatisticsAsync(
            gender, dateFilter, dateFrom, dateTo,
            ageFilter, ageMin, ageMax,
            status, groupId, campusId));

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
