using QuranSchool.Api.Models;
using QuranSchool.Api.Repositories.Interfaces;
using QuranSchool.Api.Services.Interfaces;

namespace QuranSchool.Api.Services.Implementations;

public class GroupService(IGroupRepository repository) : Service<Group>(repository), IGroupService
{
    public async Task<List<Group>> GetByAcademicYearAsync(int academicYearId)
        => await repository.GetByAcademicYearAsync(academicYearId);

    public async Task<List<Group>> GetByTeacherAsync(int teacherId)
        => await repository.GetByTeacherAsync(teacherId);
}
