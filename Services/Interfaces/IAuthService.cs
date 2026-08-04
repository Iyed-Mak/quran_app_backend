using QuranSchool.Api.DTOs.Auth;

namespace QuranSchool.Api.Services.Interfaces;

public interface IAuthService
{
    Task<AuthResponse> LoginAsync(LoginRequest request);
    Task LogoutAsync(string token);
    Task ChangePasswordAsync(int userId, string role, ChangePasswordRequest request);
    Task<string> ForgotPasswordAsync(string username);
    Task ResetPasswordAsync(ResetPasswordRequest request);
    Task<bool> IsTokenRevokedAsync(string jwtId);
}
