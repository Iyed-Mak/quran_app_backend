using QuranSchool.Api.Models;

namespace QuranSchool.Api.Services.Interfaces;

public interface IRequiredDocumentService : IService<RequiredDocument>
{
    Task<List<RequiredDocument>> GetRequiredOnlyAsync();
}
