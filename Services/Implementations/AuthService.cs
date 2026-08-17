using Microsoft.EntityFrameworkCore;
using QuranSchool.Api.Data;
using QuranSchool.Api.DTOs.Auth;
using QuranSchool.Api.Exceptions;
using QuranSchool.Api.Models;
using QuranSchool.Api.Services.Interfaces;
using System.Security.Cryptography;

namespace QuranSchool.Api.Services.Implementations;

public class AuthService(AppDbContext context, IJwtTokenService jwtTokenService) : IAuthService
{
    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var (user, role) = await FindUserAsync(request.Username);

        if (user is null || role is null || !VerifyPassword(request.Password, user.Password))
        {
            throw new UnauthorizedException("Invalid username or password.");
        }

        var (token, expiresAt) = jwtTokenService.GenerateToken(user.Id, user.Username, role);

        return new AuthResponse
        {
            Token = token,
            ExpiresAt = expiresAt,
            UserId = user.Id,
            Role = role,
            Username = user.Username,
            FullName = user.FullName
        };
    }

    public async Task LogoutAsync(string token)
    {
        var (jwtId, expiresAt) = jwtTokenService.ReadToken(token);
        if (jwtId is null)
        {
            return;
        }

        context.RevokedTokens.RemoveRange(
            context.RevokedTokens.Where(r => r.ExpiresAt < DateTime.UtcNow));

        context.RevokedTokens.Add(new RevokedToken
        {
            JwtId = jwtId,
            Token = token,
            RevokedAt = DateTime.UtcNow,
            ExpiresAt = expiresAt ?? DateTime.UtcNow.AddHours(1)
        });

        await context.SaveChangesAsync();
    }

    public async Task ChangePasswordAsync(int userId, string role, ChangePasswordRequest request)
    {
        var user = await FindUserByIdAsync(userId, role);
        if (user is null)
        {
            throw new NotFoundException("User not found.");
        }

        if (!VerifyPassword(request.CurrentPassword, user.Password))
        {
            throw new UnauthorizedException("Current password is incorrect.");
        }

        user.Password = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
        user.UpdatedAt = DateTime.UtcNow;

        await context.SaveChangesAsync();
    }

    public async Task<string> ForgotPasswordAsync(string username)
    {
        var (user, role) = await FindUserAsync(username);
        if (user is null || role is null)
        {
            throw new NotFoundException("No account found for this username.");
        }

        var resetToken = Convert.ToHexString(RandomNumberGenerator.GetBytes(32));

        context.PasswordResetTokens.Add(new PasswordResetToken
        {
            Username = user.Username,
            Role = role,
            Token = resetToken,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddMinutes(30)
        });

        await context.SaveChangesAsync();

        return resetToken;
    }

    public async Task ResetPasswordAsync(ResetPasswordRequest request)
    {
        var reset = await context.PasswordResetTokens
            .FirstOrDefaultAsync(p => p.Token == request.Token);

        if (reset is null || reset.UsedAt is not null)
        {
            throw new BadRequestException("Invalid or already-used reset token.");
        }

        if (reset.ExpiresAt < DateTime.UtcNow)
        {
            throw new BadRequestException("Reset token has expired.");
        }

        var (user, role) = await FindUserAsync(reset.Username);
        if (user is null || role != reset.Role)
        {
            throw new BadRequestException("Invalid reset token.");
        }

        user.Password = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
        user.UpdatedAt = DateTime.UtcNow;
        reset.UsedAt = DateTime.UtcNow;

        await context.SaveChangesAsync();
    }

    public async Task<bool> IsTokenRevokedAsync(string jwtId)
        => await context.RevokedTokens.AnyAsync(r => r.JwtId == jwtId);

    private static bool VerifyPassword(string plain, string hash)
    {
        try
        {
            return BCrypt.Net.BCrypt.Verify(plain, hash);
        }
        catch
        {
            return false;
        }
    }

    private async Task<(IUserAccount? User, string? Role)> FindUserAsync(string username)
    {
        var admin = await context.Admins.FirstOrDefaultAsync(a => a.Username == username && a.IsActive);
        if (admin is not null)
        {
            return (admin, "admin");
        }

        var teacher = await context.Teachers.FirstOrDefaultAsync(t => t.Username == username && t.IsActive);
        if (teacher is not null)
        {
            return (teacher, "teacher");
        }

        var parent = await context.Parents.FirstOrDefaultAsync(p => p.Username == username);
        if (parent is not null)
        {
            return (parent, "parent");
        }

        var student = await context.Students.FirstOrDefaultAsync(s => s.Username == username);
        if (student is not null)
        {
            if (student.Status == "suspended")
            {
                throw new ForbiddenException(
                    "هذا الطالب مفصول.",
                    student.SeparationReason ?? "لم يتم تحديد السبب.");
            }
            return (student, "student");
        }

        return (null, null);
    }

    private async Task<IUserAccount?> FindUserByIdAsync(int userId, string role)
        => role switch
        {
            "admin" => await context.Admins.FirstOrDefaultAsync(a => a.Id == userId && a.IsActive),
            "teacher" => await context.Teachers.FirstOrDefaultAsync(t => t.Id == userId && t.IsActive),
            "parent" => await context.Parents.FindAsync(userId),
            "student" => await context.Students.FindAsync(userId),
            _ => null
        };
}
