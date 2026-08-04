using Microsoft.EntityFrameworkCore;
using QuranSchool.Api.Data;
using QuranSchool.Api.Models;
using QuranSchool.Api.Repositories.Interfaces;

namespace QuranSchool.Api.Repositories.Implementations;

public class StudentRepository(AppDbContext context) : Repository<Student>(context), IStudentRepository
{
    public async Task<Student?> GetByUsernameAsync(string username)
        => await _context.Students.FirstOrDefaultAsync(s => s.Username == username);

    public async Task<List<Student>> GetByGroupAsync(int groupId)
        => await _context.Students
            .Where(s => s.GroupId == groupId)
            .AsNoTracking()
            .ToListAsync();

    public async Task<List<Student>> GetByParentAsync(int parentId)
        => await _context.Students
            .Where(s => s.ParentId == parentId)
            .AsNoTracking()
            .ToListAsync();

    public async Task<int> GetMaxStudentNumberAsync()
        => await _context.Students.MaxAsync(s => (int?)s.StudentNumber) ?? 0;
}
