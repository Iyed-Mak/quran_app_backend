using QuranSchool.Api.DTOs.Statistics;

namespace QuranSchool.Api.Services.Interfaces;

public interface IStatisticsService
{
    Task<OverviewStatisticsResponse> GetOverviewAsync();
    Task<StudentStatisticsResponse> GetStudentStatisticsAsync(
        string? gender, string? dateFilter, DateOnly? dateFrom, DateOnly? dateTo,
        string? ageFilter, int? ageMin, int? ageMax,
        string? status, int? groupId, int? campusId);
    Task<RegistrationStatisticsResponse> GetRegistrationStatisticsAsync(
        string? period, DateOnly? dateFrom, DateOnly? dateTo);
    Task<GroupStatisticsResponse> GetGroupStatisticsAsync();
    Task<TeacherStatisticsResponse> GetTeacherStatisticsAsync();
    Task<CampusStatisticsResponse> GetCampusStatisticsAsync();
    Task<RoomStatisticsResponse> GetRoomStatisticsAsync();
    Task<AttendanceStatisticsResponse> GetAttendanceStatisticsAsync(
        string? period, DateOnly? dateFrom, DateOnly? dateTo);
    Task<AcademicStatisticsResponse> GetAcademicStatisticsAsync();
    Task<ExamStatisticsResponse> GetExamStatisticsAsync(
        int? semesterId, int? groupId, string? gender);
}
