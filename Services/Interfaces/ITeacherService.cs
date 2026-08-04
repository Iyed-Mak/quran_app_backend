using QuranSchool.Api.DTOs.Teacher;
using QuranSchool.Api.Models;

namespace QuranSchool.Api.Services.Interfaces;

public interface ITeacherService : IService<Teacher>
{
    Task<Teacher?> GetByUsernameAsync(string username);
    Task<Teacher> CreateAsync(CreateTeacherRequest request);
    Task UpdateAsync(int id, UpdateTeacherRequest request);
}
