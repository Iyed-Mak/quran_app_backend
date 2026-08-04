using System.ComponentModel.DataAnnotations.Schema;

namespace QuranSchool.Api.Models;

[Table("revoked_token")]
public class RevokedToken : IEntity
{
    public int Id { get; set; }
    public string JwtId { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public DateTime RevokedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
}
