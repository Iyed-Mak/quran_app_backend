using System.ComponentModel.DataAnnotations.Schema;

namespace QuranSchool.Api.Models;

[Table("academic_year")]
public class AcademicYear : IEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public bool IsCurrent { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public List<Semester> Semesters { get; set; } = new();
    public List<Group> Groups { get; set; } = new();
    public List<ExamPlan> ExamPlans { get; set; } = new();
    public List<StudySchedule> StudySchedules { get; set; } = new();
}
