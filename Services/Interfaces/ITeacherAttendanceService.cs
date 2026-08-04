using QuranSchool.Api.Models;

namespace QuranSchool.Api.Services.Interfaces;

public interface ITeacherAttendanceService : IService<TeacherAttendance>
{
    Task<List<TeacherAttendance>> GetByTeacherAsync(int teacherId);
    Task<List<TeacherAttendance>> GetByGroupAsync(int groupId);
}
