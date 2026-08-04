using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace QuranSchool.Api.Models;

[Table("exam_result")]
public class ExamResult : IEntity
{
    public int Id { get; set; }
    public int ExamId { get; set; }
    public int StudentId { get; set; }
    public decimal? ExamGrade { get; set; }
    public decimal? ContinuousEvaluation { get; set; }
    public decimal? FinalGrade { get; set; }
    public ExamResultStatus Status { get; set; } = ExamResultStatus.Pending;

    [ValidateNever]
    public Exam Exam { get; set; } = null!;
    [ValidateNever]
    public Student Student { get; set; } = null!;

    public bool IsConfirmed => Status == ExamResultStatus.Confirmed;
}
