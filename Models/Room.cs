using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace QuranSchool.Api.Models;

[Table("room")]
public class Room : IEntity
{
    public int Id { get; set; }
    public int CampusId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int? Capacity { get; set; }
    public string? Description { get; set; }

    [ValidateNever]
    public Campus Campus { get; set; } = null!;
    public List<Exam> Exams { get; set; } = new();
    public List<StudySchedule> StudySchedules { get; set; } = new();
}
