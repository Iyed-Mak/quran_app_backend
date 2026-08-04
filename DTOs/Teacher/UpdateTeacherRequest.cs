using System.ComponentModel.DataAnnotations;

namespace QuranSchool.Api.DTOs.Teacher;

/// <summary>بيانات تعديل أستاذ. كلمة المرور تُترك فارغة للإبقاء على الحالية،
/// وتُرسل قيمة جديدة لتغييرها (تُشفَّر bcrypt قبل الحفظ).</summary>
public class UpdateTeacherRequest
{
    public int? RegistrationNumber { get; set; }

    [Required(ErrorMessage = "الاسم الكامل مطلوب.")]
    [StringLength(150, MinimumLength = 3, ErrorMessage = "الاسم الكامل يجب أن يكون بين 3 و150 حرفًا.")]
    public string FullName { get; set; } = string.Empty;

    public bool IsFemale { get; set; }

    public string? Phone { get; set; }

    public DateOnly? DateOfBirth { get; set; }

    [Required(ErrorMessage = "اسم المستخدم مطلوب.")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "اسم المستخدم يجب أن يكون بين 3 و50 حرفًا.")]
    public string Username { get; set; } = string.Empty;

    /// <summary>فارغة = الإبقاء على كلمة المرور الحالية.</summary>
    public string Password { get; set; } = string.Empty;
}
