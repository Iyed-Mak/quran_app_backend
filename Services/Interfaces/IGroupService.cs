using QuranSchool.Api.Models;

namespace QuranSchool.Api.Services.Interfaces;

public interface IGroupService : IService<Group>
{
    Task<List<Group>> GetByAcademicYearAsync(int academicYearId);
    Task<List<Group>> GetByTeacherAsync(int teacherId);
}
