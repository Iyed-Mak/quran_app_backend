using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using QuranSchool.Api.DTOs.Auth;
using QuranSchool.Api.Services.Interfaces;

namespace QuranSchool.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService) : ControllerBase
{
    [AllowAnonymous]
    [HttpPost("login")]
    [EnableRateLimiting("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
        => Ok(await authService.LoginAsync(request));

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        var token = ReadBearerToken();
        if (token is not null)
        {
            await authService.LogoutAsync(token);
        }

        return NoContent();
    }

    [Authorize]
    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        await authService.ChangePasswordAsync(GetUserId(), GetRole(), request);
        return NoContent();
    }

    [AllowAnonymous]
    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
    {
        var resetToken = await authService.ForgotPasswordAsync(request.Username);
        return Ok(new
        {
            Message = "Password reset token generated. In production this would be emailed to the user.",
            ResetToken = resetToken
        });
    }

    [AllowAnonymous]
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
    {
        await authService.ResetPasswordAsync(request);
        return Ok(new { Message = "Password has been reset. You can now login with the new password." });
    }

    [Authorize]
    [HttpGet("me")]
    public IActionResult Me()
        => Ok(new
        {
            UserId = GetUserId(),
            Username = User.Identity?.Name,
            Role = GetRole()
        });

    private int GetUserId()
        => int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : 0;

    private string GetRole()
        => User.FindFirstValue(ClaimTypes.Role) ?? string.Empty;

    private string? ReadBearerToken()
    {
        var header = Request.Headers.Authorization.ToString();
        return header.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)
            ? header["Bearer ".Length..]
            : null;
    }
}
