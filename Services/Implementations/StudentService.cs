using Microsoft.EntityFrameworkCore;
using QuranSchool.Api.DTOs.Student;
using QuranSchool.Api.Exceptions;
using QuranSchool.Api.Models;
using QuranSchool.Api.Repositories.Interfaces;
using QuranSchool.Api.Services.Interfaces;

namespace QuranSchool.Api.Services.Implementations;

public class StudentService(IStudentRepository repository) : Service<Student>(repository), IStudentService
{
    public async Task<Student?> GetByUsernameAsync(string username)
        => await repository.GetByUsernameAsync(username);

    public async Task<List<Student>> GetByGroupAsync(int groupId)
        => await repository.GetByGroupAsync(groupId);

    public async Task<List<Student>> GetByParentAsync(int parentId)
        => await repository.GetByParentAsync(parentId);

    public async Task<Student> CreateAsync(CreateStudentRequest request)
    {
        if (!request.DateOfBirth.HasValue || request.DateOfBirth == DateOnly.MinValue)
        {
            throw new BadRequestException("تاريخ الميلاد مطلوب.");
        }

        if (string.IsNullOrWhiteSpace(request.Password) || request.Password.Length < 6)
        {
            throw new BadRequestException("كلمة المرور يجب ألا تقل عن 6 أحرف.");
        }

        var existing = await repository.GetByUsernameAsync(request.Username);
        if (existing is not null)
        {
            throw new BadRequestException("اسم المستخدم مستخدم بالفعل.");
        }

        var studentNumber = request.StudentNumber ?? 0;
        if (studentNumber <= 0)
        {
            studentNumber = await repository.GetMaxStudentNumberAsync() + 1;
        }

        var serialNumber = string.IsNullOrWhiteSpace(request.SerialNumber)
            ? $"QS-{DateTime.UtcNow.Year}-{studentNumber}"
            : request.SerialNumber.Trim();

        var now = DateTime.UtcNow;
        var student = new Student
        {
            ParentId = request.ParentId,
            GroupId = request.GroupId,
            StudentNumber = studentNumber,
            SerialNumber = serialNumber,
            FullName = request.FullName.Trim(),
            IsFemale = request.IsFemale,
            DateOfBirth = request.DateOfBirth.Value,
            OldMemorization = request.OldMemorization,
            StudentPhone = request.StudentPhone,
            Username = request.Username.Trim(),
            Password = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Status = string.IsNullOrWhiteSpace(request.Status) ? "active" : request.Status.Trim(),
            SeparationReason = request.SeparationReason,
            CreatedAt = now,
            UpdatedAt = now
        };

        return await base.CreateAsync(student);
    }

    public async Task<Student> UpdateAsync(int id, UpdateStudentRequest request)
    {
        var student = await repository.GetByIdAsync(id);
        if (student is null)
        {
            throw new NotFoundException("الطالب غير موجود.");
        }

        if (!request.DateOfBirth.HasValue || request.DateOfBirth == DateOnly.MinValue)
        {
            throw new BadRequestException("تاريخ الميلاد مطلوب.");
        }

        var existing = await repository.GetByUsernameAsync(request.Username);
        if (existing is not null && existing.Id != id)
        {
            throw new BadRequestException("اسم المستخدم مستخدم بالفعل.");
        }

        var hashedPassword = string.IsNullOrWhiteSpace(request.Password)
            ? student.Password
            : BCrypt.Net.BCrypt.HashPassword(request.Password);

        student.ParentId = request.ParentId;
        student.GroupId = request.GroupId;
        student.StudentNumber = request.StudentNumber ?? student.StudentNumber;
        student.SerialNumber = string.IsNullOrWhiteSpace(request.SerialNumber)
            ? student.SerialNumber
            : request.SerialNumber.Trim();
        student.FullName = request.FullName.Trim();
        student.IsFemale = request.IsFemale;
        student.DateOfBirth = request.DateOfBirth.Value;
        student.OldMemorization = request.OldMemorization;
        student.StudentPhone = request.StudentPhone;
        student.Username = request.Username.Trim();
        student.Password = hashedPassword;
        student.Status = string.IsNullOrWhiteSpace(request.Status) ? student.Status : request.Status.Trim();
        student.SeparationReason = request.SeparationReason ?? student.SeparationReason;
        student.UpdatedAt = DateTime.UtcNow;

        await base.UpdateAsync(student);
        return student;
    }

    public async Task<string> ResetPasswordAsync(int id, string? newPassword)
    {
        var student = await repository.GetByIdAsync(id);
        if (student is null)
        {
            throw new NotFoundException("الطالب غير موجود.");
        }

        var password = string.IsNullOrWhiteSpace(newPassword)
            ? GeneratePassword()
            : newPassword.Trim();

        if (password.Length < 6)
        {
            throw new BadRequestException("كلمة المرور يجب ألا تقل عن 6 أحرف.");
        }

        student.Password = BCrypt.Net.BCrypt.HashPassword(password);
        student.UpdatedAt = DateTime.UtcNow;
        await base.UpdateAsync(student);
        return password;
    }

    public async Task<Student> UpdateStatusAsync(int id, UpdateStudentStatusRequest request)
    {
        var student = await repository.GetByIdAsync(id);
        if (student is null)
        {
            throw new NotFoundException("الطالب غير موجود.");
        }

        var newStatus = request.Status?.Trim().ToLower();
        if (newStatus != "active" && newStatus != "suspended")
        {
            throw new BadRequestException("قيمة الحالة غير صالحة. يجب أن تكون 'active' أو 'suspended'.");
        }

        if (newStatus == "suspended" && string.IsNullOrWhiteSpace(request.SeparationReason))
        {
            throw new BadRequestException("سبب الفصل مطلوب عند تعليق الحساب.");
        }

        student.Status = newStatus;
        student.SeparationReason = newStatus == "suspended" ? request.SeparationReason?.Trim() : null;
        student.UpdatedAt = DateTime.UtcNow;

        await base.UpdateAsync(student);
        return student;
    }

    private static string GeneratePassword()
    {
        const string upper = "ABCDEFGHJKLMNPQRSTUVWXYZ";
        const string lower = "abcdefghjkmnpqrstuvwxyz";
        const string digits = "23456789";
        const string all = upper + lower + digits;
        var rng = new Random();
        var length = 6 + rng.Next(3);
        var chars = new char[length];
        chars[0] = upper[rng.Next(upper.Length)];
        chars[1] = lower[rng.Next(lower.Length)];
        chars[2] = digits[rng.Next(digits.Length)];
        for (var i = 3; i < length; i++)
        {
            chars[i] = all[rng.Next(all.Length)];
        }

        return new string(chars.OrderBy(_ => rng.Next()).ToArray());
    }
}
