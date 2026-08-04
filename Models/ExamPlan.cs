using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace QuranSchool.Api.Models;

[Table("exam_plan")]
public class ExamPlan : IEntity
{
    public int Id { get; set; }
    public int AcademicYearId { get; set; }
    public int SemesterId { get; set; }
    public string Title { get; set; } = string.Empty;
    public int CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }

    [ValidateNever]
    public AcademicYear AcademicYear { get; set; } = null!;
    [ValidateNever]
    public Semester Semester { get; set; } = null!;
    [ValidateNever]
    public Admin Creator { get; set; } = null!;
    public List<Exam> Exams { get; set; } = new();
}
