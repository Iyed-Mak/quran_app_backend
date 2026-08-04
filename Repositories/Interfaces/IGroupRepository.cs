using QuranSchool.Api.Models;

namespace QuranSchool.Api.Repositories.Interfaces;

public interface IGroupRepository : IRepository<Group>
{
    Task<List<Group>> GetByAcademicYearAsync(int academicYearId);
    Task<List<Group>> GetByTeacherAsync(int teacherId);
}
