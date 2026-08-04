using QuranSchool.Api.Models;

namespace QuranSchool.Api.Repositories.Interfaces;

public interface IAcademicYearRepository : IRepository<AcademicYear>
{
    Task<AcademicYear?> GetCurrentAsync();
}
