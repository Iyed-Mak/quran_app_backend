namespace QuranSchool.Api.DTOs.Statistics;

// ── 1. Overview ──
public class OverviewStatisticsResponse
{
    public int TotalStudents { get; set; }
    public int MaleStudents { get; set; }
    public int FemaleStudents { get; set; }
    public int TotalTeachers { get; set; }
    public int MaleTeachers { get; set; }
    public int FemaleTeachers { get; set; }
    public int TotalGroups { get; set; }
    public int TotalCampuses { get; set; }
    public int TotalRooms { get; set; }
    public int SuspendedStudents { get; set; }
}

// ── 2. Student Statistics ──
public class StudentStatisticsResponse
{
    public int TotalCount { get; set; }
    public List<StudentListItem> Students { get; set; } = new();
}

public class StudentListItem
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public bool IsFemale { get; set; }
    public int? Age { get; set; }
    public string Status { get; set; } = string.Empty;
    public int? GroupId { get; set; }
    public string? GroupName { get; set; }
    public int? CampusId { get; set; }
    public string? CampusName { get; set; }
    public DateTime CreatedAt { get; set; }
}

// ── 3. Registration Statistics ──
public class RegistrationStatisticsResponse
{
    public List<RegistrationDataPoint> DataPoints { get; set; } = new();
    public int TotalRegistrations { get; set; }
}

public class RegistrationDataPoint
{
    public string Label { get; set; } = string.Empty;
    public int Count { get; set; }
}

// ── 4. Group Statistics ──
public class GroupStatisticsResponse
{
    public List<GroupDetail> Groups { get; set; } = new();
    public int GroupsWithoutTeacher { get; set; }
    public int LargeGroups { get; set; }
    public int SmallGroups { get; set; }
}

public class GroupDetail
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsFemale { get; set; }
    public string? TeacherName { get; set; }
    public int StudentCount { get; set; }
}

// ── 5. Teacher Statistics ──
public class TeacherStatisticsResponse
{
    public int TotalTeachers { get; set; }
    public int MaleTeachers { get; set; }
    public int FemaleTeachers { get; set; }
    public int TeachersWithoutGroups { get; set; }
    public double AvgStudentsPerTeacher { get; set; }
    public double AvgGroupsPerTeacher { get; set; }
    public List<TeacherDetail> Teachers { get; set; } = new();
}

public class TeacherDetail
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public bool IsFemale { get; set; }
    public int GroupCount { get; set; }
    public int StudentCount { get; set; }
}

// ── 6. Campus Statistics ──
public class CampusStatisticsResponse
{
    public int TotalCampuses { get; set; }
    public List<CampusDetail> Campuses { get; set; } = new();
}

public class CampusDetail
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int RoomCount { get; set; }
    public int GroupCount { get; set; }
    public int StudentCount { get; set; }
    public int TeacherCount { get; set; }
}

// ── 7. Room Statistics ──
public class RoomStatisticsResponse
{
    public int TotalRooms { get; set; }
    public int OccupiedRooms { get; set; }
    public int EmptyRooms { get; set; }
    public List<RoomDetail> Rooms { get; set; } = new();
}

public class RoomDetail
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string CampusName { get; set; } = string.Empty;
    public bool IsOccupied { get; set; }
    public string? GroupName { get; set; }
    public bool? GroupIsFemale { get; set; }
    public string? Weekday { get; set; }
    public string? TimeSlot { get; set; }
}

// ── 8. Attendance Statistics ──
public class AttendanceStatisticsResponse
{
    public int TotalPresent { get; set; }
    public int TotalAbsent { get; set; }
    public double AttendanceRate { get; set; }
    public double AbsenceRate { get; set; }
    public List<GroupAttendanceDetail> ByGroup { get; set; } = new();
}

public class GroupAttendanceDetail
{
    public int GroupId { get; set; }
    public string GroupName { get; set; } = string.Empty;
    public int Present { get; set; }
    public int Absent { get; set; }
    public double AttendanceRate { get; set; }
}

// ── 9. Academic / Quran Statistics ──
public class AcademicStatisticsResponse
{
    public double AvgEvaluation { get; set; }
    public double AvgMemorization { get; set; }
    public double AvgReview { get; set; }
    public int StudentsNeedingFollowUp { get; set; }
    public double AvgExamResult { get; set; }
    public List<GroupAcademicDetail> ByGroup { get; set; } = new();
}

public class GroupAcademicDetail
{
    public int GroupId { get; set; }
    public string GroupName { get; set; } = string.Empty;
    public double AvgEvaluation { get; set; }
    public double AvgMemorization { get; set; }
    public double AvgReview { get; set; }
}

// ── 10. Exam Statistics ──
public class ExamStatisticsResponse
{
    public int TotalExams { get; set; }
    public int UpcomingExams { get; set; }
    public int CompletedExams { get; set; }
    public double AvgResults { get; set; }
    public double HighestResult { get; set; }
    public double LowestResult { get; set; }
}
