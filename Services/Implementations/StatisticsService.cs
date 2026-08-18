using Microsoft.EntityFrameworkCore;
using QuranSchool.Api.Data;
using QuranSchool.Api.DTOs.Statistics;
using QuranSchool.Api.Services.Interfaces;

namespace QuranSchool.Api.Services.Implementations;

public class StatisticsService(AppDbContext context) : IStatisticsService
{
    public async Task<OverviewStatisticsResponse> GetOverviewAsync()
    {
        var now = DateOnly.FromDateTime(DateTime.UtcNow);

        var totalStudents = await context.Students.CountAsync();
        var maleStudents = await context.Students.CountAsync(s => !s.IsFemale && s.Status == "active");
        var femaleStudents = await context.Students.CountAsync(s => s.IsFemale && s.Status == "active");
        var totalTeachers = await context.Teachers.CountAsync(t => t.IsActive);
        var maleTeachers = await context.Teachers.CountAsync(t => !t.IsFemale && t.IsActive);
        var femaleTeachers = await context.Teachers.CountAsync(t => t.IsFemale && t.IsActive);
        var totalGroups = await context.Groups.CountAsync();
        var totalCampuses = await context.Campuses.CountAsync();
        var totalRooms = await context.Rooms.CountAsync();
        var suspendedStudents = await context.Students.CountAsync(s => s.Status == "separated");

        return new OverviewStatisticsResponse
        {
            TotalStudents = totalStudents,
            MaleStudents = maleStudents,
            FemaleStudents = femaleStudents,
            TotalTeachers = totalTeachers,
            MaleTeachers = maleTeachers,
            FemaleTeachers = femaleTeachers,
            TotalGroups = totalGroups,
            TotalCampuses = totalCampuses,
            TotalRooms = totalRooms,
            SuspendedStudents = suspendedStudents
        };
    }

    public async Task<StudentStatisticsResponse> GetStudentStatisticsAsync(
        string? gender, string? dateFilter, DateOnly? dateFrom, DateOnly? dateTo,
        string? ageFilter, int? ageMin, int? ageMax,
        string? status, int? groupId, int? campusId)
    {
        var now = DateOnly.FromDateTime(DateTime.UtcNow);
        var query = context.Students
            .AsNoTracking()
            .Include(s => s.Group)
                .ThenInclude(g => g!.StudySchedules)
                    .ThenInclude(ss => ss.Campus)
            .AsQueryable();

        if (!string.IsNullOrEmpty(gender))
        {
            if (gender == "male") query = query.Where(s => !s.IsFemale);
            else if (gender == "female") query = query.Where(s => s.IsFemale);
        }

        if (!string.IsNullOrEmpty(status))
            query = query.Where(s => s.Status == status);

        if (groupId.HasValue)
            query = query.Where(s => s.GroupId == groupId.Value);

        if (campusId.HasValue)
            query = query.Where(s => s.Group != null &&
                s.Group.StudySchedules.Any(ss => ss.CampusId == campusId.Value));

        DateOnly? filterDateFrom = null;
        DateOnly? filterDateTo = null;

        if (!string.IsNullOrEmpty(dateFilter))
        {
            filterDateFrom = dateFilter switch
            {
                "today" => now,
                "week" => now.AddDays(-(int)now.DayOfWeek),
                "month" => new DateOnly(now.Year, now.Month, 1),
                "year" => new DateOnly(now.Year, 1, 1),
                _ => dateFrom
            };
            filterDateTo = dateFilter == "custom" ? dateTo : now;
        }

        if (filterDateFrom.HasValue)
            query = query.Where(s => s.CreatedAt >= filterDateFrom.Value.ToDateTime(TimeOnly.MinValue));
        if (filterDateTo.HasValue)
            query = query.Where(s => s.CreatedAt <= filterDateTo.Value.ToDateTime(TimeOnly.MaxValue));

        var students = await query.ToListAsync();

        if (!string.IsNullOrEmpty(ageFilter))
        {
            students = ageFilter switch
            {
                "under10" => students.Where(s => s.DateOfBirth > now.AddYears(-10)).ToList(),
                "10to15" => students.Where(s => s.DateOfBirth <= now.AddYears(-10) && s.DateOfBirth > now.AddYears(-15)).ToList(),
                "over15" => students.Where(s => s.DateOfBirth <= now.AddYears(-15)).ToList(),
                "over16" => students.Where(s => s.DateOfBirth <= now.AddYears(-16)).ToList(),
                "custom" when ageMin.HasValue && ageMax.HasValue =>
                    students.Where(s =>
                        s.DateOfBirth <= now.AddYears(-ageMin.Value) &&
                        s.DateOfBirth > now.AddYears(-ageMax.Value - 1)).ToList(),
                _ => students
            };
        }

        var campusLookup = context.StudySchedules
            .AsNoTracking()
            .GroupBy(ss => ss.GroupId)
            .Select(g => new { GroupId = g.Key, CampusId = g.First().CampusId, CampusName = g.First().Campus!.Name })
            .ToDictionaryAsync(x => x.GroupId);

        var campusMap = await campusLookup;

        return new StudentStatisticsResponse
        {
            TotalCount = students.Count,
            Students = students.Select(s => new StudentListItem
            {
                Id = s.Id,
                FullName = s.FullName,
                IsFemale = s.IsFemale,
                Age = now.Year - s.DateOfBirth.Year - (now.DayOfYear < s.DateOfBirth.DayOfYear ? 1 : 0),
                Status = s.Status,
                GroupId = s.GroupId,
                GroupName = s.Group?.Name,
                CampusId = s.Group != null && campusMap.ContainsKey(s.GroupId ?? 0) ? campusMap[s.GroupId!.Value].CampusId : null,
                CampusName = s.Group != null && campusMap.ContainsKey(s.GroupId ?? 0) ? campusMap[s.GroupId!.Value].CampusName : null,
                CreatedAt = s.CreatedAt
            }).ToList()
        };
    }

    public async Task<RegistrationStatisticsResponse> GetRegistrationStatisticsAsync(
        string? period, DateOnly? dateFrom, DateOnly? dateTo)
    {
        var now = DateOnly.FromDateTime(DateTime.UtcNow);
        var start = period switch
        {
            "month" => new DateOnly(now.Year, now.Month, 1),
            "year" => new DateOnly(now.Year, 1, 1),
            "custom" when dateFrom.HasValue => dateFrom.Value,
            _ => new DateOnly(now.Year, 1, 1)
        };
        var end = period == "custom" && dateTo.HasValue ? dateTo.Value : now;

        var students = await context.Students
            .AsNoTracking()
            .Where(s => s.CreatedAt >= start.ToDateTime(TimeOnly.MinValue) && s.CreatedAt <= end.ToDateTime(TimeOnly.MaxValue))
            .ToListAsync();

        var grouped = students
            .GroupBy(s => s.CreatedAt.ToString("yyyy-MM"))
            .OrderBy(g => g.Key)
            .Select(g => new RegistrationDataPoint { Label = g.Key, Count = g.Count() })
            .ToList();

        return new RegistrationStatisticsResponse
        {
            TotalRegistrations = students.Count,
            DataPoints = grouped
        };
    }

    public async Task<GroupStatisticsResponse> GetGroupStatisticsAsync()
    {
        var groups = await context.Groups
            .AsNoTracking()
            .Include(g => g.Teacher)
            .Include(g => g.Students)
            .ToListAsync();

        var details = groups.Select(g => new GroupDetail
        {
            Id = g.Id,
            Name = g.Name,
            IsFemale = g.IsFemale,
            TeacherName = g.Teacher?.FullName,
            StudentCount = g.Students.Count
        }).ToList();

        var avgCount = details.Any() ? details.Average(d => d.StudentCount) : 0;

        return new GroupStatisticsResponse
        {
            Groups = details.OrderByDescending(d => d.StudentCount).ToList(),
            GroupsWithoutTeacher = details.Count(d => string.IsNullOrEmpty(d.TeacherName)),
            LargeGroups = details.Count(d => d.StudentCount > avgCount * 1.5),
            SmallGroups = details.Count(d => d.StudentCount < 3 && d.StudentCount > 0)
        };
    }

    public async Task<TeacherStatisticsResponse> GetTeacherStatisticsAsync()
    {
        var teachers = await context.Teachers
            .AsNoTracking()
            .Where(t => t.IsActive)
            .Include(t => t.Groups)
                .ThenInclude(g => g!.Students)
            .ToListAsync();

        var details = teachers.Select(t => new TeacherDetail
        {
            Id = t.Id,
            FullName = t.FullName,
            IsFemale = t.IsFemale,
            GroupCount = t.Groups.Count,
            StudentCount = t.Groups.SelectMany(g => g!.Students).Count()
        }).ToList();

        var total = details.Count;
        var totalStudents = details.Sum(d => d.StudentCount);
        var totalGroups = details.Sum(d => d.GroupCount);

        return new TeacherStatisticsResponse
        {
            TotalTeachers = total,
            MaleTeachers = details.Count(d => !d.IsFemale),
            FemaleTeachers = details.Count(d => d.IsFemale),
            TeachersWithoutGroups = details.Count(d => d.GroupCount == 0),
            AvgStudentsPerTeacher = total > 0 ? Math.Round((double)totalStudents / total, 1) : 0,
            AvgGroupsPerTeacher = total > 0 ? Math.Round((double)totalGroups / total, 1) : 0,
            Teachers = details.OrderByDescending(d => d.StudentCount).ToList()
        };
    }

    public async Task<CampusStatisticsResponse> GetCampusStatisticsAsync()
    {
        var campuses = await context.Campuses
            .AsNoTracking()
            .Include(c => c.Rooms)
            .ToListAsync();

        var campusIds = campuses.Select(c => c.Id).ToList();

        var scheduleData = await context.StudySchedules
            .AsNoTracking()
            .Where(ss => campusIds.Contains(ss.CampusId))
            .GroupBy(ss => ss.CampusId)
            .Select(g => new
            {
                CampusId = g.Key,
                GroupCount = g.Select(ss => ss.GroupId).Distinct().Count(),
                StudentCount = g.SelectMany(ss => ss.Group!.Students).Count(),
                TeacherCount = g.Select(ss => ss.Group!.TeacherId).Distinct().Count()
            })
            .ToDictionaryAsync(x => x.CampusId);

        var details = campuses.Select(c =>
        {
            scheduleData.TryGetValue(c.Id, out var data);
            return new CampusDetail
            {
                Id = c.Id,
                Name = c.Name,
                RoomCount = c.Rooms.Count,
                GroupCount = data?.GroupCount ?? 0,
                StudentCount = data?.StudentCount ?? 0,
                TeacherCount = data?.TeacherCount ?? 0
            };
        }).ToList();

        return new CampusStatisticsResponse
        {
            TotalCampuses = campuses.Count,
            Campuses = details
        };
    }

    public async Task<RoomStatisticsResponse> GetRoomStatisticsAsync()
    {
        var rooms = await context.Rooms
            .AsNoTracking()
            .Include(r => r.Campus)
            .ToListAsync();

        var occupiedRoomIds = await context.StudySchedules
            .AsNoTracking()
            .Select(ss => ss.RoomId)
            .Distinct()
            .ToListAsync();

        var roomGroupMap = await context.StudySchedules
            .AsNoTracking()
            .GroupBy(ss => ss.RoomId)
            .Select(g => new { RoomId = g.Key, GroupName = g.First().Group!.Name })
            .ToDictionaryAsync(x => x.RoomId, x => x.GroupName);

        var details = rooms.Select(r =>
        {
            var isOccupied = occupiedRoomIds.Contains(r.Id);
            roomGroupMap.TryGetValue(r.Id, out var groupName);
            return new RoomDetail
            {
                Id = r.Id,
                Name = r.Name,
                CampusName = r.Campus?.Name ?? string.Empty,
                IsOccupied = isOccupied,
                GroupName = groupName
            };
        }).ToList();

        var occupiedCount = occupiedRoomIds.Count;

        return new RoomStatisticsResponse
        {
            TotalRooms = rooms.Count,
            OccupiedRooms = occupiedCount,
            EmptyRooms = rooms.Count - occupiedCount,
            Rooms = details
        };
    }

    public async Task<AttendanceStatisticsResponse> GetAttendanceStatisticsAsync(
        string? period, DateOnly? dateFrom, DateOnly? dateTo)
    {
        var now = DateOnly.FromDateTime(DateTime.UtcNow);
        var start = period switch
        {
            "today" => now,
            "week" => now.AddDays(-(int)now.DayOfWeek),
            "month" => new DateOnly(now.Year, now.Month, 1),
            "custom" when dateFrom.HasValue => dateFrom.Value,
            _ => now.AddDays(-7)
        };
        var end = period == "custom" && dateTo.HasValue ? dateTo.Value : now;

        var evaluations = await context.DailyEvaluations
            .AsNoTracking()
            .Where(e => e.SessionDate >= start && e.SessionDate <= end)
            .Include(e => e.Student)
                .ThenInclude(s => s!.Group)
            .ToListAsync();

        var present = evaluations.Count(e => e.Attendance == "present");
        var absent = evaluations.Count(e => e.Attendance == "absent");
        var total = present + absent;

        var byGroup = evaluations
            .Where(e => e.Student?.Group != null)
            .GroupBy(e => e.Student!.Group!)
            .Select(g =>
            {
                var p = g.Count(e => e.Attendance == "present");
                var a = g.Count(e => e.Attendance == "absent");
                var t = p + a;
                return new GroupAttendanceDetail
                {
                    GroupId = g.Key.Id,
                    GroupName = g.Key.Name,
                    Present = p,
                    Absent = a,
                    AttendanceRate = t > 0 ? Math.Round((double)p / t * 100, 1) : 0
                };
            })
            .OrderByDescending(d => d.AttendanceRate)
            .ToList();

        return new AttendanceStatisticsResponse
        {
            TotalPresent = present,
            TotalAbsent = absent,
            AttendanceRate = total > 0 ? Math.Round((double)present / total * 100, 1) : 0,
            AbsenceRate = total > 0 ? Math.Round((double)absent / total * 100, 1) : 0,
            ByGroup = byGroup
        };
    }

    public async Task<AcademicStatisticsResponse> GetAcademicStatisticsAsync()
    {
        var evaluations = await context.DailyEvaluations
            .AsNoTracking()
            .Include(e => e.Student)
                .ThenInclude(s => s!.Group)
            .ToListAsync();

        var avgEvaluation = evaluations.Where(e => e.Evaluation.HasValue)
            .Select(e => (double)e.Evaluation!.Value)
            .DefaultIfEmpty(0)
            .Average();

        var avgMemorization = 0.0;
        var avgReview = 0.0;
        var needingFollowUp = 0;

        var memValues = evaluations.Where(e => !string.IsNullOrEmpty(e.NewMemorization))
            .Select(e => double.TryParse(e.NewMemorization, out var v) ? v : (double?)null)
            .Where(v => v.HasValue)
            .Select(v => v!.Value)
            .ToList();
        if (memValues.Any()) avgMemorization = Math.Round(memValues.Average(), 2);

        var revValues = evaluations.Where(e => !string.IsNullOrEmpty(e.ReviewQuantity))
            .Select(e => double.TryParse(e.ReviewQuantity, out var v) ? v : (double?)null)
            .Where(v => v.HasValue)
            .Select(v => v!.Value)
            .ToList();
        if (revValues.Any()) avgReview = Math.Round(revValues.Average(), 2);

        var examResults = await context.ExamResults
            .AsNoTracking()
            .Where(e => e.FinalGrade.HasValue)
            .ToListAsync();

        var avgExam = examResults.Any()
            ? Math.Round((double)examResults.Average(e => e.FinalGrade!.Value), 2)
            : 0;

        var lowEvalStudents = evaluations
            .Where(e => e.Evaluation.HasValue && e.Evaluation < 10)
            .Select(e => e.StudentId)
            .Distinct()
            .Count();

        var lowExamStudents = examResults
            .Where(e => e.FinalGrade.HasValue && e.FinalGrade < 10)
            .Select(e => e.StudentId)
            .Distinct()
            .Count();

        needingFollowUp = lowEvalStudents + lowExamStudents;

        var byGroup = evaluations
            .Where(e => e.Student?.Group != null)
            .GroupBy(e => e.Student!.Group!)
            .Select(g => new GroupAcademicDetail
            {
                GroupId = g.Key.Id,
                GroupName = g.Key.Name,
                AvgEvaluation = Math.Round(g.Where(e => e.Evaluation.HasValue).Select(e => (double)e.Evaluation!.Value).DefaultIfEmpty(0).Average(), 2),
                AvgMemorization = Math.Round(
                    g.Where(e => !string.IsNullOrEmpty(e.NewMemorization))
                     .Select(e => double.TryParse(e.NewMemorization, out var v) ? v : (double?)null)
                     .Where(v => v.HasValue).Select(v => v!.Value)
                     .DefaultIfEmpty(0).Average(), 2),
                AvgReview = Math.Round(
                    g.Where(e => !string.IsNullOrEmpty(e.ReviewQuantity))
                     .Select(e => double.TryParse(e.ReviewQuantity, out var v) ? v : (double?)null)
                     .Where(v => v.HasValue).Select(v => v!.Value)
                     .DefaultIfEmpty(0).Average(), 2)
            })
            .ToList();

        return new AcademicStatisticsResponse
        {
            AvgEvaluation = Math.Round(avgEvaluation, 2),
            AvgMemorization = avgMemorization,
            AvgReview = avgReview,
            StudentsNeedingFollowUp = needingFollowUp,
            AvgExamResult = avgExam,
            ByGroup = byGroup
        };
    }

    public async Task<ExamStatisticsResponse> GetExamStatisticsAsync(
        int? semesterId, int? groupId, string? gender)
    {
        var query = context.Exams
            .AsNoTracking()
            .Include(e => e.Results)
            .Include(e => e.Group)
            .AsQueryable();

        if (semesterId.HasValue)
        {
            query = query.Where(e => e.ExamPlan.SemesterId == semesterId.Value);
        }

        if (groupId.HasValue)
            query = query.Where(e => e.GroupId == groupId.Value);

        if (!string.IsNullOrEmpty(gender))
        {
            if (gender == "male")
                query = query.Where(e => e.Group != null && !e.Group.IsFemale);
            else if (gender == "female")
                query = query.Where(e => e.Group != null && e.Group.IsFemale);
        }

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var exams = await query.ToListAsync();

        var completed = exams.Where(e => e.ExamDate <= today).ToList();
        var upcoming = exams.Where(e => e.ExamDate > today).ToList();

        var allResults = completed.SelectMany(e => e.Results).ToList();
        var grades = allResults.Where(r => r.FinalGrade.HasValue).Select(r => (double)r.FinalGrade!.Value).ToList();

        return new ExamStatisticsResponse
        {
            TotalExams = exams.Count,
            UpcomingExams = upcoming.Count,
            CompletedExams = completed.Count,
            AvgResults = grades.Any() ? Math.Round(grades.Average(), 2) : 0,
            HighestResult = grades.Any() ? grades.Max() : 0,
            LowestResult = grades.Any() ? grades.Min() : 0
        };
    }
}
