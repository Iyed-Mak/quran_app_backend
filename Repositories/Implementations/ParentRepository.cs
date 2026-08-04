using Microsoft.EntityFrameworkCore;
using QuranSchool.Api.Data;
using QuranSchool.Api.Models;
using QuranSchool.Api.Repositories.Interfaces;

namespace QuranSchool.Api.Repositories.Implementations;

public class ParentRepository(AppDbContext context) : Repository<Parent>(context), IParentRepository
{
    public async Task<Parent?> GetByUsernameAsync(string username)
        => await _context.Parents.FirstOrDefaultAsync(p => p.Username == username);
}
