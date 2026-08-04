using QuranSchool.Api.Models;

namespace QuranSchool.Api.Services.Interfaces;

public interface ISemesterService : IService<Semester>
{
    Task<List<Semester>> GetByAcademicYearAsync(int academicYearId);
}
