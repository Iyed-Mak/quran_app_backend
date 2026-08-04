using Microsoft.EntityFrameworkCore;
using QuranSchool.Api.Data;
using QuranSchool.Api.Models;
using QuranSchool.Api.Repositories.Interfaces;

namespace QuranSchool.Api.Repositories.Implementations;

public class AdminRepository(AppDbContext context) : Repository<Admin>(context), IAdminRepository
{
    public async Task<Admin?> GetByUsernameAsync(string username)
        => await _context.Admins.FirstOrDefaultAsync(a => a.Username == username);
}
