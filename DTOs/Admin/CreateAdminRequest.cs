using System.ComponentModel.DataAnnotations;

namespace QuranSchool.Api.DTOs.Admin;

/// <summary>بيانات إنشاء مسؤول جديد. كلمة المرور تُخزَّن بعد تشفيرها (bcrypt).</summary>
public class CreateAdminRequest
{
    [Required(ErrorMessage = "الاسم الكامل مطلوب.")]
    [StringLength(150, MinimumLength = 3, ErrorMessage = "الاسم الكامل يجب أن يكون بين 3 و150 حرفًا.")]
    public string FullName { get; set; } = string.Empty;

    public string? Phone { get; set; }

    [Required(ErrorMessage = "اسم المستخدم مطلوب.")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "اسم المستخدم يجب أن يكون بين 3 و50 حرفًا.")]
    public string Username { get; set; } = string.Empty;

    [MinLength(6, ErrorMessage = "كلمة المرور يجب ألا تقل عن 6 أحرف.")]
    public string Password { get; set; } = string.Empty;
}
