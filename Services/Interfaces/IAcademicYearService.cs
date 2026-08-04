using QuranSchool.Api.Models;

namespace QuranSchool.Api.Services.Interfaces;

public interface IAcademicYearService : IService<AcademicYear>
{
    Task<AcademicYear?> GetCurrentAsync();
}
