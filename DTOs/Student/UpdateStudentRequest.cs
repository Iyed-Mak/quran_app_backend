using System.ComponentModel.DataAnnotations;

namespace QuranSchool.Api.DTOs.Student;

/// <summary>
/// بيانات تعديل طالب. إذا تُرك `Password` فارغًا أو null تُحفَظ كلمة المرور
/// الحالية دون تغيير.
/// </summary>
public class UpdateStudentRequest
{
    public int? ParentId { get; set; }

    public int? GroupId { get; set; }

    public int? StudentNumber { get; set; }

    public string? SerialNumber { get; set; }

    [Required(ErrorMessage = "الاسم الكامل مطلوب.")]
    [StringLength(150, MinimumLength = 3, ErrorMessage = "الاسم الكامل يجب أن يكون بين 3 و150 حرفًا.")]
    public string FullName { get; set; } = string.Empty;

    public bool IsFemale { get; set; }

    public DateOnly? DateOfBirth { get; set; }

    public string? OldMemorization { get; set; }

    public string? StudentPhone { get; set; }

    [Required(ErrorMessage = "اسم المستخدم مطلوب.")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "اسم المستخدم يجب أن يكون بين 3 و50 حرفًا.")]
    public string Username { get; set; } = string.Empty;

    /// <summary>فارغ/null = إبقاء كلمة المرور الحالية.</summary>
    public string? Password { get; set; }
}
