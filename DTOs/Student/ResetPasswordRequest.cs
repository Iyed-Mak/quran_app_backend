namespace QuranSchool.Api.DTOs.Student;

/// <summary>
/// طلب إعادة تعيين كلمة مرور الطالب. إذا تُرك `NewPassword` فارغًا
/// تولِّد الخدمة كلمة مرور عشوائية وتُعيدها نصية (مرة واحدة فقط).
/// </summary>
public class ResetPasswordRequest
{
    public string? NewPassword { get; set; }
}
