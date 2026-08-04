using Microsoft.EntityFrameworkCore;
using QuranSchool.Api.Data;
using QuranSchool.Api.Models;
using QuranSchool.Api.Repositories.Interfaces;

namespace QuranSchool.Api.Repositories.Implementations;

public class StudentDocumentRepository(AppDbContext context) : Repository<StudentDocument>(context), IStudentDocumentRepository
{
    public async Task<List<StudentDocument>> GetByStudentAsync(int studentId)
        => await _context.StudentDocuments
            .Where(d => d.StudentId == studentId)
            .AsNoTracking()
            .ToListAsync();

    public async Task<List<StudentDocument>> GetMissingAsync()
        => await _context.StudentDocuments
            .Where(d => !d.IsSubmitted)
            .AsNoTracking()
            .ToListAsync();
}
