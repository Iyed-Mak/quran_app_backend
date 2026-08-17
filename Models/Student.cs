using System.ComponentModel.DataAnnotations.Schema;

namespace QuranSchool.Api.Models;

[Table("student")]
public class Student : IUserAccount
{
    public int Id { get; set; }
    public int? ParentId { get; set; }
    public int? GroupId { get; set; }
    public int StudentNumber { get; set; }
    public string SerialNumber { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public bool IsFemale { get; set; }
    public DateOnly DateOfBirth { get; set; }
    public string? OldMemorization { get; set; }
    public string? StudentPhone { get; set; }
    public string Username { get; set; } = string.Empty;

    [System.Text.Json.Serialization.JsonIgnore]
    public string Password { get; set; } = string.Empty;
    public string Status { get; set; } = "active";
    public string? SeparationReason { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public Parent? Parent { get; set; }
    public Group? Group { get; set; }
    public List<ExamResult> ExamResults { get; set; } = new();
    public List<DailyEvaluation> DailyEvaluations { get; set; } = new();
    public List<Homework> Homeworks { get; set; } = new();
    public List<StudentDocument> StudentDocuments { get; set; } = new();
}
