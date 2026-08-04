using QuranSchool.Api.DTOs.Admin;
using QuranSchool.Api.Exceptions;
using QuranSchool.Api.Models;
using QuranSchool.Api.Repositories.Interfaces;
using QuranSchool.Api.Services.Interfaces;

namespace QuranSchool.Api.Services.Implementations;

public class AdminService(IAdminRepository repository) : Service<Admin>(repository), IAdminService
{
    public async Task<Admin?> GetByUsernameAsync(string username)
        => await repository.GetByUsernameAsync(username);

    public override async Task<List<Admin>> GetAllAsync()
    {
        var all = await base.GetAllAsync();
        return all.Where(a => a.IsActive).ToList();
    }

    public async Task<Admin> CreateAsync(CreateAdminRequest request)
    {
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
        var admin = new Admin
        {
            FullName = request.FullName.Trim(),
            Phone = request.Phone?.Trim() ?? string.Empty,
            Username = request.Username.Trim(),
            Password = BCrypt.Net.BCrypt.HashPassword(request.Password),
            CreatedAt = now,
            UpdatedAt = now,
            IsActive = true
        };

        return await base.CreateAsync(admin);
    }

    public override async Task UpdateAsync(Admin entity)
    {
        if (string.IsNullOrWhiteSpace(entity.FullName) || entity.FullName.Trim().Length < 3)
        {
            throw new BadRequestException("الاسم الكامل يجب ألا يقل عن 3 أحرف.");
        }

        if (string.IsNullOrWhiteSpace(entity.Username) || entity.Username.Trim().Length < 3)
        {
            throw new BadRequestException("اسم المستخدم يجب ألا يقل عن 3 أحرف.");
        }

        var existing = await repository.GetByIdAsync(entity.Id);
        if (existing is null)
        {
            return;
        }

        var byUsername = await repository.GetByUsernameAsync(entity.Username);
        if (byUsername is not null && byUsername.Id != entity.Id)
        {
            throw new BadRequestException("اسم المستخدم مستخدم بالفعل.");
        }

        entity.Password = existing.Password;
        // `is_active` قد لا يُرسَل من الواجهة، لذلك نحافظ على القيمة
        // الحالية كي لا يُعطَّل الحساب بمجرد تعديل بياناته.
        entity.IsActive = existing.IsActive;
        entity.FullName = entity.FullName.Trim();
        entity.Phone = entity.Phone?.Trim() ?? string.Empty;
        entity.Username = entity.Username.Trim();
        entity.UpdatedAt = DateTime.UtcNow;
        await base.UpdateAsync(entity);
    }

    public override async Task DeleteAsync(int id)
    {
        var entity = await repository.GetByIdAsync(id);
        if (entity is null)
        {
            return;
        }

        var all = await GetAllAsync();
        if (all.Count <= 1)
        {
            throw new BadRequestException("لا يمكن حذف آخر حساب مسؤول في النظام.");
        }

        // حذف ناعم: نُعطّل الحساب بدل حذفه نهائيًا حتى لا تنكسر
        // القيود المرجعية (خطط الامتحانات) ولا تُفقد البيانات التاريخية.
        entity.IsActive = false;
        entity.UpdatedAt = DateTime.UtcNow;
        _repository.Update(entity);
        await _repository.SaveChangesAsync();
    }
}
