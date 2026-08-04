using Microsoft.EntityFrameworkCore;
using QuranSchool.Api.Data;
using QuranSchool.Api.Models;
using QuranSchool.Api.Repositories.Interfaces;

namespace QuranSchool.Api.Repositories.Implementations;

public class HomeworkRepository(AppDbContext context) : Repository<Homework>(context), IHomeworkRepository
{
    public async Task<List<Homework>> GetByStudentAsync(int studentId)
        => await _context.Homeworks
            .Where(h => h.StudentId == studentId)
            .AsNoTracking()
            .ToListAsync();

    public async Task<List<Homework>> GetByTeacherAsync(int teacherId)
        => await _context.Homeworks
            .Where(h => h.TeacherId == teacherId)
            .AsNoTracking()
            .ToListAsync();
}
