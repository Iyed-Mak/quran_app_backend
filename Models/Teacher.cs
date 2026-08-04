using System.ComponentModel.DataAnnotations.Schema;

namespace QuranSchool.Api.Models;

[Table("teacher")]
public class Teacher : IUserAccount
{
    public int Id { get; set; }

    /// <summary>الرقم التسلسلي للمعلم (مستقل عن معرّف قاعدة البيانات).
    /// يُولَّد تلقائيًا عند الإنشاء بقيمة أعلى رقم + 1.</summary>
    public int RegistrationNumber { get; set; }

    public string FullName { get; set; } = string.Empty;
    public bool IsFemale { get; set; }
    public string Phone { get; set; } = string.Empty;
    public DateOnly DateOfBirth { get; set; }
    public string Username { get; set; } = string.Empty;

    [System.Text.Json.Serialization.JsonIgnore]
    public string Password { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    /// <summary>يدل على أن الحساب مفعّل. عند الحذف نُعطّل الحساب بدل حذف
    /// السجلات المرتبطة (حضور، تقييمات، مجموعات...) للحفاظ على البيانات.</summary>
    public bool IsActive { get; set; } = true;

    public List<Group> Groups { get; set; } = new();
    public List<Exam> Exams { get; set; } = new();
    public List<DailyEvaluation> DailyEvaluations { get; set; } = new();
    public List<Homework> Homeworks { get; set; } = new();
    public List<TeacherAttendance> TeacherAttendances { get; set; } = new();
}
