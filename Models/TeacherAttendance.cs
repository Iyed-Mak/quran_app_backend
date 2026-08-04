using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace QuranSchool.Api.Models;

[Table("teacher_attendance")]
public class TeacherAttendance : IEntity
{
    public int Id { get; set; }
    public int TeacherId { get; set; }
    public int GroupId { get; set; }
    public DateOnly SessionDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Justification { get; set; }
    public DateTime CreatedAt { get; set; }

    [JsonIgnore, ValidateNever]
    public Teacher Teacher { get; set; } = null!;

    [JsonIgnore, ValidateNever]
    public Group Group { get; set; } = null!;
}
