using QuranSchool.Api.Models;
using QuranSchool.Api.Repositories.Interfaces;
using QuranSchool.Api.Services.Interfaces;

namespace QuranSchool.Api.Services.Implementations;

public class SemesterService(ISemesterRepository repository) : Service<Semester>(repository), ISemesterService
{
    public async Task<List<Semester>> GetByAcademicYearAsync(int academicYearId)
        => await repository.GetByAcademicYearAsync(academicYearId);
}
