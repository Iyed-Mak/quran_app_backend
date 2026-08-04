using QuranSchool.Api.DTOs.Parent;
using QuranSchool.Api.Models;

namespace QuranSchool.Api.Services.Interfaces;

public interface IParentService : IService<Parent>
{
    Task<Parent?> GetByUsernameAsync(string username);
    Task<Parent> CreateAsync(CreateParentRequest request);
}
