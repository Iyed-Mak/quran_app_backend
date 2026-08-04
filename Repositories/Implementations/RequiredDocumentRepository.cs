using Microsoft.EntityFrameworkCore;
using QuranSchool.Api.Data;
using QuranSchool.Api.Models;
using QuranSchool.Api.Repositories.Interfaces;

namespace QuranSchool.Api.Repositories.Implementations;

public class RequiredDocumentRepository(AppDbContext context) : Repository<RequiredDocument>(context), IRequiredDocumentRepository
{
    public async Task<List<RequiredDocument>> GetRequiredOnlyAsync()
        => await _context.RequiredDocuments.Where(d => d.IsRequired).AsNoTracking().ToListAsync();

    public async Task DeleteIncludingStudentDocumentsAsync(int requiredDocumentId)
    {
        var entity = await _context.RequiredDocuments.FindAsync(requiredDocumentId);
        if (entity is null)
        {
            return;
        }

        var related = await _context.StudentDocuments
            .Where(sd => sd.RequiredDocumentId == requiredDocumentId)
            .ToListAsync();
        _context.StudentDocuments.RemoveRange(related);
        _context.RequiredDocuments.Remove(entity);
        await _context.SaveChangesAsync();
    }
}
