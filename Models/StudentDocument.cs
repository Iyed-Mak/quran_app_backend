using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace QuranSchool.Api.Models;

[Table("student_document")]
public class StudentDocument : IEntity
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public int RequiredDocumentId { get; set; }
    public bool IsSubmitted { get; set; }
    public DateTime? SubmittedAt { get; set; }
    public string? Notes { get; set; }

    [ValidateNever]
    public Student Student { get; set; } = null!;
    [ValidateNever]
    public RequiredDocument RequiredDocument { get; set; } = null!;
}
