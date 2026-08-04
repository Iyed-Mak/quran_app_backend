using System.ComponentModel.DataAnnotations.Schema;

namespace QuranSchool.Api.Models;

[Table("admin")]
public class Admin : IUserAccount
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;

    [System.Text.Json.Serialization.JsonIgnore]
    public string Password { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    /// <summary>يدل على أن الحساب مفعّل. عند الحذف نُعطّل الحساب بدل حذف
    /// السجلات المرتبطة (خطط الامتحانات...) للحفاظ على البيانات التاريخية.</summary>
    public bool IsActive { get; set; } = true;

    public List<ExamPlan> ExamPlans { get; set; } = new();
}
