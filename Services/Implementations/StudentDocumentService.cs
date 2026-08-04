using QuranSchool.Api.DTOs.StudentDocument;
using QuranSchool.Api.Exceptions;
using QuranSchool.Api.Models;
using QuranSchool.Api.Repositories.Interfaces;
using QuranSchool.Api.Services.Interfaces;

namespace QuranSchool.Api.Services.Implementations;

public class StudentDocumentService(IStudentDocumentRepository repository) : Service<StudentDocument>(repository), IStudentDocumentService
{
    public async Task<List<StudentDocument>> GetByStudentAsync(int studentId)
        => await repository.GetByStudentAsync(studentId);

    public async Task<List<StudentDocument>> GetMissingAsync()
        => await repository.GetMissingAsync();

    public async Task<StudentDocument> CreateAsync(CreateStudentDocumentRequest request)
    {
        var now = DateTime.UtcNow;
        var document = new StudentDocument
        {
            StudentId = request.StudentId,
            RequiredDocumentId = request.RequiredDocumentId,
            IsSubmitted = request.IsSubmitted,
            SubmittedAt = request.IsSubmitted ? request.SubmittedAt ?? now : null,
            Notes = request.Notes
        };

        return await base.CreateAsync(document);
    }

    public async Task UpdateAsync(int id, UpdateStudentDocumentRequest request)
    {
        var document = await repository.GetByIdAsync(id);
        if (document is null)
        {
            throw new NotFoundException("المستند غير موجود.");
        }

        document.StudentId = request.StudentId;
        document.RequiredDocumentId = request.RequiredDocumentId;
        document.IsSubmitted = request.IsSubmitted;
        document.SubmittedAt = request.IsSubmitted ? request.SubmittedAt : null;
        document.Notes = request.Notes;

        await base.UpdateAsync(document);
    }
}
