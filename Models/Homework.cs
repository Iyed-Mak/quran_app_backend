using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace QuranSchool.Api.Models;

[Table("homework")]
public class Homework : IEntity
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public int TeacherId { get; set; }
    public DateOnly SessionDate { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? Notes { get; set; }

    [ValidateNever]
    public Student Student { get; set; } = null!;
    [ValidateNever]
    public Teacher Teacher { get; set; } = null!;
}
