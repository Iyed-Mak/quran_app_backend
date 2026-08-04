using QuranSchool.Api.Models;

namespace QuranSchool.Api.Repositories.Interfaces;

public interface IRequiredDocumentRepository : IRepository<RequiredDocument>
{
    Task<List<RequiredDocument>> GetRequiredOnlyAsync();

    Task DeleteIncludingStudentDocumentsAsync(int requiredDocumentId);
}
