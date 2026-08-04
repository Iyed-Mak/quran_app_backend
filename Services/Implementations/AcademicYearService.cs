using QuranSchool.Api.Models;
using QuranSchool.Api.Repositories.Interfaces;
using QuranSchool.Api.Services.Interfaces;

namespace QuranSchool.Api.Services.Implementations;

public class AcademicYearService(IAcademicYearRepository repository) : Service<AcademicYear>(repository), IAcademicYearService
{
    public async Task<AcademicYear?> GetCurrentAsync()
        => await repository.GetCurrentAsync();
}
