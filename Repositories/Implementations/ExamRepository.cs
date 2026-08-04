using Microsoft.EntityFrameworkCore;
using QuranSchool.Api.Data;
using QuranSchool.Api.Models;
using QuranSchool.Api.Repositories.Interfaces;

namespace QuranSchool.Api.Repositories.Implementations;

public class ExamRepository(AppDbContext context) : Repository<Exam>(context), IExamRepository
{
    public async Task<List<Exam>> GetByExamPlanAsync(int examPlanId)
        => await _context.Exams
            .Where(e => e.ExamPlanId == examPlanId)
            .AsNoTracking()
            .ToListAsync();

    public async Task<List<Exam>> GetByGroupAsync(int groupId)
        => await _context.Exams
            .Where(e => e.GroupId == groupId)
            .AsNoTracking()
            .ToListAsync();

    public async Task<bool> ExistsForCampusAsync(int campusId)
        => await _context.Exams.AnyAsync(e => e.CampusId == campusId);

    public async Task<bool> ExistsForRoomAsync(int roomId)
        => await _context.Exams.AnyAsync(e => e.RoomId == roomId);
}
