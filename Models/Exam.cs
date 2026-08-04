using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace QuranSchool.Api.Models;

[Table("exam")]
public class Exam : IEntity
{
    public int Id { get; set; }
    public string ExamType { get; set; } = string.Empty;
    public int ExamPlanId { get; set; }
    public int GroupId { get; set; }
    public int TeacherId { get; set; }
    public int CampusId { get; set; }
    public int RoomId { get; set; }
    public DateOnly ExamDate { get; set; }
    public string TimeSlot { get; set; } = string.Empty;
    public int? Duration { get; set; }
    public string? Notes { get; set; }

    [ValidateNever]
    public ExamPlan ExamPlan { get; set; } = null!;
    [ValidateNever]
    public Group Group { get; set; } = null!;
    [ValidateNever]
    public Teacher Teacher { get; set; } = null!;
    [ValidateNever]
    public Campus Campus { get; set; } = null!;
    [ValidateNever]
    public Room Room { get; set; } = null!;
    public List<ExamResult> Results { get; set; } = new();
}
