namespace QuranSchool.Api.Models;

public interface IUserAccount : IEntity
{
    string FullName { get; set; }
    string Username { get; set; }
    string Password { get; set; }
    DateTime CreatedAt { get; set; }
    DateTime UpdatedAt { get; set; }
}
