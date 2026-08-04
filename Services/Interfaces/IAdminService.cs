using QuranSchool.Api.DTOs.Admin;
using QuranSchool.Api.Models;

namespace QuranSchool.Api.Services.Interfaces;

public interface IAdminService : IService<Admin>
{
    Task<Admin?> GetByUsernameAsync(string username);
    Task<Admin> CreateAsync(CreateAdminRequest request);
}
