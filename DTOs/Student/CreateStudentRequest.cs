using System.ComponentModel.DataAnnotations;

namespace QuranSchool.Api.DTOs.Student;

/// <summary>بيانات إنشاء طالب جديد. كلمة المرور تُخزَّن بعد تشفيرها (bcrypt).</summary>
public class CreateStudentRequest
{
    public int? ParentId { get; set; }

    public int? GroupId { get; set; }

    /// <summary>يُولَّد تلقائيًا إذا لم يُمرَّر أو كان صفرًا.</summary>
    public int? StudentNumber { get; set; }

    /// <summary>يُولَّد تلقائيًا إذا تُرك فارغًا.</summary>
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

    [MinLength(6, ErrorMessage = "كلمة المرور يجب ألا تقل عن 6 أحرف.")]
    public string Password { get; set; } = string.Empty;
}
