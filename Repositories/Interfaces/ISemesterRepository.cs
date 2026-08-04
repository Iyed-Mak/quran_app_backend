using QuranSchool.Api.Models;

namespace QuranSchool.Api.Repositories.Interfaces;

public interface ISemesterRepository : IRepository<Semester>
{
    Task<List<Semester>> GetByAcademicYearAsync(int academicYearId);
}
