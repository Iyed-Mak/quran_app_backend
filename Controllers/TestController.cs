using Microsoft.AspNetCore.Mvc;
using QuranSchool.Api.Services.Interfaces;

namespace QuranSchool.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TestController : ControllerBase
{
    private readonly IStudentService _students;
    private readonly IGroupService _groups;
    private readonly IAcademicYearService _academicYears;
    private readonly IExamResultService _examResults;
    private readonly INotificationReceiverService _receivers;

    public TestController(
        IStudentService students,
        IGroupService groups,
        IAcademicYearService academicYears,
        IExamResultService examResults,
        INotificationReceiverService receivers)
    {
        _students = students;
        _groups = groups;
        _academicYears = academicYears;
        _examResults = examResults;
        _receivers = receivers;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var currentYear = await _academicYears.GetCurrentAsync();
        var groups = await _groups.GetByAcademicYearAsync(currentYear?.Id ?? 0);
        var unread = await _receivers.GetUnreadCountAsync("student", 1);

        return Ok(new
        {
            Message = "Database Connected Successfully! (via services)",
            Students = (await _students.GetAllAsync()).Count,
            Groups = groups.Count,
            ExamResults = (await _examResults.GetAllAsync()).Count,
            CurrentYear = currentYear?.Name,
            UnreadNotificationsForStudent1 = unread
        });
    }
}
