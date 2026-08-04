using QuranSchool.Api.DTOs.StudentDocument;
using QuranSchool.Api.Models;

namespace QuranSchool.Api.Services.Interfaces;

public interface IStudentDocumentService : IService<StudentDocument>
{
    Task<List<StudentDocument>> GetByStudentAsync(int studentId);
    Task<List<StudentDocument>> GetMissingAsync();
    Task<StudentDocument> CreateAsync(CreateStudentDocumentRequest request);
    Task UpdateAsync(int id, UpdateStudentDocumentRequest request);
}
