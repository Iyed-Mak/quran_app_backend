using QuranSchool.Api.Models;

namespace QuranSchool.Api.Repositories.Interfaces;

public interface ITeacherAttendanceRepository : IRepository<TeacherAttendance>
{
    Task<List<TeacherAttendance>> GetByTeacherAsync(int teacherId);
    Task<List<TeacherAttendance>> GetByGroupAsync(int groupId);
}
