using QuranSchool.Api.DTOs.Student;
using QuranSchool.Api.Models;

namespace QuranSchool.Api.Services.Interfaces;

public interface IStudentService : IService<Student>
{
    Task<Student?> GetByUsernameAsync(string username);
    Task<List<Student>> GetByGroupAsync(int groupId);
    Task<List<Student>> GetByParentAsync(int parentId);
    Task<Student> CreateAsync(CreateStudentRequest request);
    Task<Student> UpdateAsync(int id, UpdateStudentRequest request);
    Task<string> ResetPasswordAsync(int id, string? newPassword);
}
