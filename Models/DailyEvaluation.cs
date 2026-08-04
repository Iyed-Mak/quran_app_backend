using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace QuranSchool.Api.Models;

[Table("daily_evaluation")]
public class DailyEvaluation : IEntity
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public int TeacherId { get; set; }
    public DateOnly SessionDate { get; set; }
    public string Attendance { get; set; } = string.Empty;
    public string? NewMemorization { get; set; }
    public string? ReviewQuantity { get; set; }
    public decimal? Evaluation { get; set; }
    public string? TeacherNote { get; set; }
    public string? Homework { get; set; }
    public DateTime CreatedAt { get; set; }

    [ValidateNever]
    public Student Student { get; set; } = null!;
    [ValidateNever]
    public Teacher Teacher { get; set; } = null!;
}
