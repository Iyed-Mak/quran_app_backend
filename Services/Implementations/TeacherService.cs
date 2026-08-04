using QuranSchool.Api.DTOs.Teacher;
using QuranSchool.Api.Exceptions;
using QuranSchool.Api.Models;
using QuranSchool.Api.Repositories.Interfaces;
using QuranSchool.Api.Services.Interfaces;

namespace QuranSchool.Api.Services.Implementations;

public class TeacherService(ITeacherRepository repository) : Service<Teacher>(repository), ITeacherService
{
    public async Task<Teacher?> GetByUsernameAsync(string username)
        => await repository.GetByUsernameAsync(username);

    public override async Task<List<Teacher>> GetAllAsync()
    {
        var all = await base.GetAllAsync();
        return all.Where(t => t.IsActive).ToList();
    }

    public override async Task DeleteAsync(int id)
    {
        var teacher = await repository.GetByIdAsync(id);
        if (teacher is null)
        {
            return;
        }

        // حذف ناعم: نُعطّل الحساب بدل حذفه نهائيًا حتى لا تنكسر
        // القيود المرجعية (مجموعات، حضور، تقييمات، واجبات، امتحانات)
        // ولا تُفقد البيانات التاريخية.
        teacher.IsActive = false;
        teacher.UpdatedAt = DateTime.UtcNow;
        _repository.Update(teacher);
        await _repository.SaveChangesAsync();
    }

    public async Task<Teacher> CreateAsync(CreateTeacherRequest request)
    {
        if (!request.DateOfBirth.HasValue || request.DateOfBirth == DateOnly.MinValue)
        {
            throw new BadRequestException("تاريخ الميلاد مطلوب.");
        }

        if (string.IsNullOrWhiteSpace(request.Password) || request.Password.Length < 6)
        {
            throw new BadRequestException("كلمة المرور يجب ألا تقل عن 6 أحرف.");
        }

        if (string.IsNullOrWhiteSpace(request.Username) || request.Username.Trim().Length < 3)
        {
            throw new BadRequestException("اسم المستخدم يجب ألا يقل عن 3 أحرف.");
        }

        var existing = await repository.GetByUsernameAsync(request.Username);
        if (existing is not null)
        {
            throw new BadRequestException("اسم المستخدم مستخدم بالفعل.");
        }

        var now = DateTime.UtcNow;
        var teacher = new Teacher
        {
            FullName = request.FullName.Trim(),
            IsFemale = request.IsFemale,
            Phone = request.Phone?.Trim() ?? string.Empty,
            DateOfBirth = request.DateOfBirth.Value,
            Username = request.Username.Trim(),
            Password = BCrypt.Net.BCrypt.HashPassword(request.Password),
            CreatedAt = now,
            UpdatedAt = now
        };

        return await base.CreateAsync(teacher);
    }

    public override async Task UpdateAsync(Teacher entity)
    {
        var existing = await repository.GetByIdAsync(entity.Id);
        if (existing is not null)
        {
            // إن أُرسلت كلمة مرور جديدة (غير فارغة) نُشفّرها، وإلا
            // نحتفظ بالكلمة الحالية لأنها لا تُرسل في استجابات الواجهة.
            entity.Password = string.IsNullOrWhiteSpace(entity.Password)
                ? existing.Password
                : BCrypt.Net.BCrypt.HashPassword(entity.Password);
        }
        entity.UpdatedAt = DateTime.UtcNow;
        await base.UpdateAsync(entity);
    }

    public async Task UpdateAsync(int id, UpdateTeacherRequest request)
    {
        var existing = await repository.GetByIdAsync(id)
            ?? throw new NotFoundException("الأستاذ غير موجود.");

        if (string.IsNullOrWhiteSpace(request.FullName) || request.FullName.Trim().Length < 3)
        {
            throw new BadRequestException("الاسم الكامل مطلوب.");
        }

        if (request.Username is null || request.Username.Trim().Length < 3)
        {
            throw new BadRequestException("اسم المستخدم يجب ألا يقل عن 3 أحرف.");
        }

        if (!request.DateOfBirth.HasValue || request.DateOfBirth == DateOnly.MinValue)
        {
            throw new BadRequestException("تاريخ الميلاد مطلوب.");
        }

        var duplicate = await repository.GetByUsernameAsync(request.Username);
        if (duplicate is not null && duplicate.Id != id)
        {
            throw new BadRequestException("اسم المستخدم مستخدم بالفعل.");
        }

        existing.FullName = request.FullName.Trim();
        existing.IsFemale = request.IsFemale;
        existing.Phone = request.Phone?.Trim() ?? string.Empty;
        existing.DateOfBirth = request.DateOfBirth.Value;
        existing.Username = request.Username.Trim();
        if (!string.IsNullOrWhiteSpace(request.Password))
        {
            existing.Password = BCrypt.Net.BCrypt.HashPassword(request.Password);
        }
        existing.UpdatedAt = DateTime.UtcNow;

        _repository.Update(existing);
        await _repository.SaveChangesAsync();
    }
}
