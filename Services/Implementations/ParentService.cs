using QuranSchool.Api.DTOs.Parent;
using QuranSchool.Api.Exceptions;
using QuranSchool.Api.Models;
using QuranSchool.Api.Repositories.Interfaces;
using QuranSchool.Api.Services.Interfaces;

namespace QuranSchool.Api.Services.Implementations;

public class ParentService(IParentRepository repository) : Service<Parent>(repository), IParentService
{
    public async Task<Parent?> GetByUsernameAsync(string username)
        => await repository.GetByUsernameAsync(username);

    public async Task<Parent> CreateAsync(CreateParentRequest request)
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
        var parent = new Parent
        {
            FullName = request.FullName.Trim(),
            Phone = request.Phone?.Trim() ?? string.Empty,
            Username = request.Username.Trim(),
            Password = BCrypt.Net.BCrypt.HashPassword(request.Password),
            CreatedAt = now,
            UpdatedAt = now
        };

        return await base.CreateAsync(parent);
    }

    public override async Task UpdateAsync(Parent entity)
    {
        var existing = await repository.GetByIdAsync(entity.Id);
        if (existing is not null)
        {
            entity.Password = existing.Password;
        }
        entity.UpdatedAt = DateTime.UtcNow;
        await base.UpdateAsync(entity);
    }
}
