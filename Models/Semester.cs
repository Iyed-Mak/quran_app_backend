using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace QuranSchool.Api.Models;

[Table("semester")]
public class Semester : IEntity
{
    public int Id { get; set; }
    public int AcademicYearId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int OrderIndex { get; set; }
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }

    [ValidateNever]
    public AcademicYear AcademicYear { get; set; } = null!;
    public List<ExamPlan> ExamPlans { get; set; } = new();
}
