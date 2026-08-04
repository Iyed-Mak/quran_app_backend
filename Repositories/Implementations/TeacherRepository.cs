using Microsoft.EntityFrameworkCore;
using QuranSchool.Api.Data;
using QuranSchool.Api.Models;
using QuranSchool.Api.Repositories.Interfaces;

namespace QuranSchool.Api.Repositories.Implementations;

public class TeacherRepository(AppDbContext context) : Repository<Teacher>(context), ITeacherRepository
{
    public async Task<Teacher?> GetByUsernameAsync(string username)
        => await _context.Teachers.FirstOrDefaultAsync(t => t.Username == username);

    public async Task<int> GetMaxRegistrationNumberAsync()
        => await _context.Teachers.MaxAsync(t => (int?)t.RegistrationNumber) ?? 0;
}
