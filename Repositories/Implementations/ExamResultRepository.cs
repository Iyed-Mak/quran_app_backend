using Microsoft.EntityFrameworkCore;
using QuranSchool.Api.Data;
using QuranSchool.Api.Models;
using QuranSchool.Api.Repositories.Interfaces;

namespace QuranSchool.Api.Repositories.Implementations;

public class ExamResultRepository(AppDbContext context) : Repository<ExamResult>(context), IExamResultRepository
{
    public async Task<List<ExamResult>> GetByExamAsync(int examId)
        => await _context.ExamResults
            .Where(r => r.ExamId == examId)
            .AsNoTracking()
            .ToListAsync();

    public async Task<List<ExamResult>> GetByStudentAsync(int studentId)
        => await _context.ExamResults
            .Where(r => r.StudentId == studentId)
            .AsNoTracking()
            .ToListAsync();
}
