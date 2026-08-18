using System.ComponentModel.DataAnnotations.Schema;

namespace QuranSchool.Api.Models;

[Table("school_working_hours")]
public class SchoolWorkingHours : IEntity
{
    public int Id { get; set; }
    public int SchoolInformationId { get; set; }
    public string DayOfWeek { get; set; } = string.Empty;
    public bool IsOpen { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    [ForeignKey(nameof(SchoolInformationId))]
    public SchoolInformation? SchoolInformation { get; set; }

    public List<SchoolWorkingPeriod> Periods { get; set; } = new();
}
