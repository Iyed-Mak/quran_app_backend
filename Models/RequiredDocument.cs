using System.ComponentModel.DataAnnotations.Schema;

namespace QuranSchool.Api.Models;

[Table("required_document")]
public class RequiredDocument : IEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsRequired { get; set; }
    public DateTime CreatedAt { get; set; }

    public List<StudentDocument> StudentDocuments { get; set; } = new();
}
