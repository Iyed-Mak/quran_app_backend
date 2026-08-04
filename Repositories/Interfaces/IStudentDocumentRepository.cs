using QuranSchool.Api.Models;

namespace QuranSchool.Api.Repositories.Interfaces;

public interface IStudentDocumentRepository : IRepository<StudentDocument>
{
    Task<List<StudentDocument>> GetByStudentAsync(int studentId);
    Task<List<StudentDocument>> GetMissingAsync();
}
