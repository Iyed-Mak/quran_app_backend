namespace QuranSchool.Api.DTOs.StudentDocument;

/// <summary>بيانات تعديل مستند طالب.</summary>
public class UpdateStudentDocumentRequest
{
    public int StudentId { get; set; }

    public int RequiredDocumentId { get; set; }

    public bool IsSubmitted { get; set; }

    public DateTime? SubmittedAt { get; set; }

    public string? Notes { get; set; }
}
