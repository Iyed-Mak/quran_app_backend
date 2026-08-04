using System.ComponentModel.DataAnnotations;

namespace QuranSchool.Api.DTOs.Auth;

public class ForgotPasswordRequest
{
    [Required(ErrorMessage = "Username is required.")]
    public string Username { get; set; } = string.Empty;
}
