using QuranSchool.Api.Models;
using QuranSchool.Api.Repositories.Interfaces;
using QuranSchool.Api.Services.Interfaces;

namespace QuranSchool.Api.Services.Implementations;

public class TeacherAttendanceService(ITeacherAttendanceRepository repository) : Service<TeacherAttendance>(repository), ITeacherAttendanceService
{
    public async Task<List<TeacherAttendance>> GetByTeacherAsync(int teacherId)
        => await repository.GetByTeacherAsync(teacherId);

    public async Task<List<TeacherAttendance>> GetByGroupAsync(int groupId)
        => await repository.GetByGroupAsync(groupId);
}
