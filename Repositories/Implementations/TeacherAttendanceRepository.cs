using Microsoft.EntityFrameworkCore;
using QuranSchool.Api.Data;
using QuranSchool.Api.Models;
using QuranSchool.Api.Repositories.Interfaces;

namespace QuranSchool.Api.Repositories.Implementations;

public class TeacherAttendanceRepository(AppDbContext context) : Repository<TeacherAttendance>(context), ITeacherAttendanceRepository
{
    public async Task<List<TeacherAttendance>> GetByTeacherAsync(int teacherId)
        => await _context.TeacherAttendances
            .Where(a => a.TeacherId == teacherId)
            .AsNoTracking()
            .ToListAsync();

    public async Task<List<TeacherAttendance>> GetByGroupAsync(int groupId)
        => await _context.TeacherAttendances
            .Where(a => a.GroupId == groupId)
            .AsNoTracking()
            .ToListAsync();
}
