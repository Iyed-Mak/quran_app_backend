using System.ComponentModel.DataAnnotations;

namespace QuranSchool.Api.DTOs.SchoolInformation;

public class UpdateSchoolInfoRequest
{
    [Required(ErrorMessage = "اسم المدرسة مطلوب")]
    [StringLength(200)]
    public string SchoolName { get; set; } = string.Empty;

    [Required(ErrorMessage = "وصف المدرسة مطلوب")]
    [StringLength(1000)]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "سنة التأسيس مطلوبة")]
    public int FoundedYear { get; set; }

    [Required(ErrorMessage = "نوع المدرسة مطلوب")]
    [StringLength(100)]
    public string SchoolType { get; set; } = string.Empty;

    [Required(ErrorMessage = "العنوان مطلوب")]
    [StringLength(300)]
    public string Address { get; set; } = string.Empty;

    [Required(ErrorMessage = "رقم الهاتف مطلوب")]
    [StringLength(30)]
    public string Phone { get; set; } = string.Empty;

    [StringLength(30)]
    public string? AdditionalPhone { get; set; }

    [StringLength(100)]
    [EmailAddress(ErrorMessage = "البريد الإلكتروني غير صالح")]
    public string? Email { get; set; }

    [StringLength(30)]
    public string? Whatsapp { get; set; }

    [StringLength(200)]
    [Url(ErrorMessage = "رابط الصفحة الرسمية غير صالح")]
    public string? OfficialPage { get; set; }
}

public class SchoolInfoResponse
{
    public int Id { get; set; }
    public string SchoolName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int FoundedYear { get; set; }
    public string SchoolType { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? AdditionalPhone { get; set; }
    public string? Email { get; set; }
    public string? Whatsapp { get; set; }
    public string? OfficialPage { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<SchoolWorkingHoursDto> WorkingHours { get; set; } = new();
    public List<SchoolRuleDto> Rules { get; set; } = new();
}

public class SchoolWorkingHoursDto
{
    public int Id { get; set; }
    public string DayOfWeek { get; set; } = string.Empty;
    public bool IsOpen { get; set; }
    public List<SchoolWorkingPeriodDto> Periods { get; set; } = new();
}

public class SchoolWorkingPeriodDto
{
    public int Id { get; set; }
    public string OpeningTime { get; set; } = string.Empty;
    public string ClosingTime { get; set; } = string.Empty;
}

public class SchoolRuleDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }
}

public class CreateWorkingPeriodRequest
{
    [Required(ErrorMessage = "وقت الفتح مطلوب")]
    public string OpeningTime { get; set; } = string.Empty;

    [Required(ErrorMessage = "وقت الإغلاق مطلوب")]
    public string ClosingTime { get; set; } = string.Empty;
}

public class UpdateWorkingHoursRequest
{
    [Required(ErrorMessage = "اليوم مطلوب")]
    public string DayOfWeek { get; set; } = string.Empty;

    public bool IsOpen { get; set; }
    public List<CreateWorkingPeriodRequest> Periods { get; set; } = new();
}

public class CreateSchoolRuleRequest
{
    [Required(ErrorMessage = "عنوان القانون مطلوب")]
    [StringLength(300)]
    public string Title { get; set; } = string.Empty;
}

public class UpdateSchoolRuleRequest
{
    [Required(ErrorMessage = "عنوان القانون مطلوب")]
    [StringLength(300)]
    public string Title { get; set; } = string.Empty;

    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; } = true;
}

public class ReorderRulesRequest
{
    [Required]
    public List<int> RuleIds { get; set; } = new();
}
