using System.ComponentModel.DataAnnotations.Schema;

namespace QuranSchool.Api.Models;

[Table("school_working_periods")]
public class SchoolWorkingPeriod : IEntity
{
    public int Id { get; set; }
    public int WorkingHoursId { get; set; }
    public string OpeningTime { get; set; } = string.Empty;
    public string ClosingTime { get; set; } = string.Empty;

    [ForeignKey(nameof(WorkingHoursId))]
    public SchoolWorkingHours? WorkingHours { get; set; }
}
