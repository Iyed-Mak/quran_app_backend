using Microsoft.AspNetCore.Mvc;
using QuranSchool.Api.Models;
using QuranSchool.Api.Services.Interfaces;

namespace QuranSchool.Api.Controllers;

[Route("api/[controller]")]
public class ExamResultsController : BaseCrudController<ExamResult, IExamResultService>
{
    public ExamResultsController(IExamResultService service) : base(service)
    {
    }

    [HttpGet("by-exam/{examId:int}")]
    public async Task<IActionResult> GetByExam(int examId)
        => Ok(await _service.GetByExamAsync(examId));

    [HttpGet("by-student/{studentId:int}")]
    public async Task<IActionResult> GetByStudent(int studentId)
        => Ok(await _service.GetByStudentAsync(studentId));
}
