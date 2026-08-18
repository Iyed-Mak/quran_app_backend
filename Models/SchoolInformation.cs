using System.ComponentModel.DataAnnotations.Schema;

namespace QuranSchool.Api.Models;

[Table("school_information")]
public class SchoolInformation : IEntity
{
    public int Id { get; set; }
    public string SchoolName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int FoundedYear { get; set; }
    public string SchoolType { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? AdditionalPhone { get; set; }
    public string? Email { get; set; }
    public string? Whatsapp { get; set; }
    public string? OfficialPage { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public List<SchoolWorkingHours> WorkingHours { get; set; } = new();
    public List<SchoolRule> Rules { get; set; } = new();
}
