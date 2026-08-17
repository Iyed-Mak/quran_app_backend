using System.ComponentModel.DataAnnotations;

namespace QuranSchool.Api.DTOs.Student;

/// <summary>
/// طلب تغيير حالة الطالب (نشط / مفصول).
/// </summary>
public class UpdateStudentStatusRequest
{
    [Required(ErrorMessage = "حالة الطالب مطلوبة.")]
    public string Status { get; set; } = "active";

    /// <summary>سبب الفصل — مطلوب فقط عند تغيير الحالة إلى "suspended".</summary>
    public string? SeparationReason { get; set; }
}
