using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace QuranSchool.Api.Models;

[Table("study_schedule")]
public class StudySchedule : IEntity
{
    public int Id { get; set; }
    public int AcademicYearId { get; set; }
    public int GroupId { get; set; }
    public int CampusId { get; set; }
    public int RoomId { get; set; }
    public string Weekday { get; set; } = string.Empty;
    public string TimeSlot { get; set; } = string.Empty;

    [ValidateNever]
    public AcademicYear AcademicYear { get; set; } = null!;
    [ValidateNever]
    public Group Group { get; set; } = null!;
    [ValidateNever]
    public Campus Campus { get; set; } = null!;
    [ValidateNever]
    public Room Room { get; set; } = null!;
}
