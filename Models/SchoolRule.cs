using System.ComponentModel.DataAnnotations.Schema;

namespace QuranSchool.Api.Models;

[Table("school_rules")]
public class SchoolRule : IEntity
{
    public int Id { get; set; }
    public int SchoolInformationId { get; set; }
    public string Title { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    [ForeignKey(nameof(SchoolInformationId))]
    public SchoolInformation? SchoolInformation { get; set; }
}
