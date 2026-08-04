namespace QuranSchool.Api.DTOs.StudentDocument;

/// <summary>بيانات إنشاء مستند طالب.</summary>
public class CreateStudentDocumentRequest
{
    public int StudentId { get; set; }

    public int RequiredDocumentId { get; set; }

    public bool IsSubmitted { get; set; }

    public DateTime? SubmittedAt { get; set; }

    public string? Notes { get; set; }
}
