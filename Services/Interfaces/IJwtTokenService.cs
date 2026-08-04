namespace QuranSchool.Api.Services.Interfaces;

public interface IJwtTokenService
{
    (string Token, DateTime ExpiresAt) GenerateToken(int userId, string username, string role);
    (string? JwtId, DateTime? ExpiresAt) ReadToken(string token);
}
