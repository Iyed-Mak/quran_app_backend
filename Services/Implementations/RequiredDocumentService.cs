using QuranSchool.Api.Models;
using QuranSchool.Api.Repositories.Interfaces;
using QuranSchool.Api.Services.Interfaces;

namespace QuranSchool.Api.Services.Implementations;

public class RequiredDocumentService(IRequiredDocumentRepository repository) : Service<RequiredDocument>(repository), IRequiredDocumentService
{
    public async Task<List<RequiredDocument>> GetRequiredOnlyAsync()
        => await repository.GetRequiredOnlyAsync();

    public override Task DeleteAsync(int id)
        => repository.DeleteIncludingStudentDocumentsAsync(id);
}
