using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace QuranSchool.Api.Models;

[Table("group")]
public class Group : IEntity
{
    public int Id { get; set; }
    public int AcademicYearId { get; set; }
    public int? TeacherId { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsFemale { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    [ValidateNever]
    public AcademicYear AcademicYear { get; set; } = null!;
    [ValidateNever]
    public Teacher? Teacher { get; set; }
    public List<Student> Students { get; set; } = new();
    public List<Exam> Exams { get; set; } = new();
    public List<StudySchedule> StudySchedules { get; set; } = new();
    public List<TeacherAttendance> TeacherAttendances { get; set; } = new();
}
