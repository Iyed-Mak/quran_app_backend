using Microsoft.EntityFrameworkCore;
using QuranSchool.Api.Data;
using QuranSchool.Api.DTOs.Statistics;
using QuranSchool.Api.Services.Interfaces;

namespace QuranSchool.Api.Services.Implementations;

public class StatisticsService(AppDbContext context) : IStatisticsService
{
    public async Task<OverviewStatisticsResponse> GetOverviewAsync()
    {
        var students = await context.Students.AsNoTracking().ToListAsync();
        var teachers = await context.Teachers.AsNoTracking().Where(t => t.IsActive).ToListAsync();

        return new OverviewStatisticsResponse
        {
            TotalStudents = students.Count,
            MaleStudents = students.Count(s => !s.IsFemale && s.Status == "active"),
            FemaleStudents = students.Count(s => s.IsFemale && s.Status == "active"),
            TotalTeachers = teachers.Count,
            MaleTeachers = teachers.Count(t => !t.IsFemale),
            FemaleTeachers = teachers.Count(t => t.IsFemale),
            TotalGroups = await context.Groups.AsNoTracking().CountAsync(),
            TotalCampuses = await context.Campuses.AsNoTracking().CountAsync(),
            TotalRooms = await context.Rooms.AsNoTracking().CountAsync(),
            SuspendedStudents = students.Count(s => s.Status == "suspended")
        };
    }

    public async Task<StudentStatisticsResponse> GetStudentStatisticsAsync(
        string? gender,
        string? ageOperator, int? ageValue,
        string? regDateOperator, DateOnly? regDate,
        string? status, int? groupId, int? campusId)
    {
        var now = DateOnly.FromDateTime(DateTime.UtcNow);

        var students = await context.Students
            .AsNoTracking()
            .Include(s => s.Group)
            .ToListAsync();

        var schedules = await context.StudySchedules
            .AsNoTracking()
            .ToListAsync();

        var campusByGroup = schedules
            .GroupBy(ss => ss.GroupId)
            .ToDictionary(g => g.Key, g => g.First());

        if (!string.IsNullOrEmpty(gender))
            students = gender == "male"
                ? students.Where(s => !s.IsFemale).ToList()
                : students.Where(s => s.IsFemale).ToList();

        if (!string.IsNullOrEmpty(status))
            students = students.Where(s => s.Status == status).ToList();

        if (groupId.HasValue)
            students = students.Where(s => s.GroupId == groupId.Value).ToList();

        if (campusId.HasValue)
        {
            var groupIds = schedules.Where(ss => ss.CampusId == campusId.Value).Select(ss => ss.GroupId).Distinct().ToHashSet();
            students = students.Where(s => s.GroupId.HasValue && groupIds.Contains(s.GroupId.Value)).ToList();
        }

        if (!string.IsNullOrEmpty(ageOperator) && ageValue.HasValue)
        {
            var n = ageValue.Value;
            students = ageOperator switch
            {
                "gt" => students.Where(s => s.DateOfBirth <= now.AddYears(-n - 1)).ToList(),
                "lt" => students.Where(s => s.DateOfBirth > now.AddYears(-n)).ToList(),
                "eq" => students.Where(s => s.DateOfBirth > now.AddYears(-n - 1) && s.DateOfBirth <= now.AddYears(-n)).ToList(),
                _ => students
            };
        }

        if (!string.IsNullOrEmpty(regDateOperator) && regDate.HasValue)
        {
            var d = regDate.Value;
            students = regDateOperator switch
            {
                "before" => students.Where(s => s.CreatedAt < d.ToDateTime(TimeOnly.MaxValue)).ToList(),
                "after" => students.Where(s => s.CreatedAt > d.ToDateTime(TimeOnly.MinValue)).ToList(),
                "on" => students.Where(s => s.CreatedAt >= d.ToDateTime(TimeOnly.MinValue) && s.CreatedAt <= d.ToDateTime(TimeOnly.MaxValue)).ToList(),
                _ => students
            };
        }

        return new StudentStatisticsResponse
        {
            TotalCount = students.Count,
            Students = students.Select(s =>
            {
                campusByGroup.TryGetValue(s.GroupId ?? 0, out var sch);
                return new StudentListItem
                {
                    Id = s.Id,
                    FullName = s.FullName,
                    IsFemale = s.IsFemale,
                    Age = now.Year - s.DateOfBirth.Year - (now.DayOfYear < s.DateOfBirth.DayOfYear ? 1 : 0),
                    Status = s.Status,
                    GroupId = s.GroupId,
                    GroupName = s.Group?.Name,
                    CampusId = sch?.CampusId,
                    CampusName = null,
                    CreatedAt = s.CreatedAt
                };
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
            .ToListAsync();

        var studentCounts = await context.Students
            .AsNoTracking()
            .Where(s => s.GroupId != null)
            .GroupBy(s => s.GroupId!.Value)
            .Select(g => new { GroupId = g.Key, Count = g.Count() })
            .ToListAsync();

        var countMap = studentCounts.ToDictionary(x => x.GroupId, x => x.Count);

        var details = groups.Select(g =>
        {
            countMap.TryGetValue(g.Id, out var cnt);
            return new GroupDetail
            {
                Id = g.Id,
                Name = g.Name,
                IsFemale = g.IsFemale,
                TeacherName = g.Teacher?.FullName,
                StudentCount = cnt
            };
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
            .ToListAsync();

        var groups = await context.Groups
            .AsNoTracking()
            .ToListAsync();

        var studentCounts = await context.Students
            .AsNoTracking()
            .Where(s => s.GroupId != null)
            .GroupBy(s => s.GroupId!.Value)
            .Select(g => new { GroupId = g.Key, Count = g.Count() })
            .ToListAsync();

        var studentCountMap = studentCounts.ToDictionary(x => x.GroupId, x => x.Count);
        var groupCountByTeacher = groups.Where(g => g.TeacherId != null).GroupBy(g => g.TeacherId!.Value)
            .ToDictionary(g => g.Key, g => g.Count());
        var studentCountByTeacher = groups.Where(g => g.TeacherId != null)
            .GroupBy(g => g.TeacherId!.Value)
            .ToDictionary(g => g.Key, g => g.Sum(gr => studentCountMap.GetValueOrDefault(gr.Id)));

        var details = teachers.Select(t =>
        {
            groupCountByTeacher.TryGetValue(t.Id, out var gc);
            studentCountByTeacher.TryGetValue(t.Id, out var sc);
            return new TeacherDetail
            {
                Id = t.Id,
                FullName = t.FullName,
                IsFemale = t.IsFemale,
                GroupCount = gc,
                StudentCount = sc
            };
        }).ToList();

        var total = details.Count;
        var totalStudents = details.Sum(d => d.StudentCount);
        var totalGroupsCount = details.Sum(d => d.GroupCount);

        return new TeacherStatisticsResponse
        {
            TotalTeachers = total,
            MaleTeachers = details.Count(d => !d.IsFemale),
            FemaleTeachers = details.Count(d => d.IsFemale),
            TeachersWithoutGroups = details.Count(d => d.GroupCount == 0),
            AvgStudentsPerTeacher = total > 0 ? Math.Round((double)totalStudents / total, 1) : 0,
            AvgGroupsPerTeacher = total > 0 ? Math.Round((double)totalGroupsCount / total, 1) : 0,
            Teachers = details.OrderByDescending(d => d.StudentCount).ToList()
        };
    }

    public async Task<CampusStatisticsResponse> GetCampusStatisticsAsync()
    {
        var campuses = await context.Campuses.AsNoTracking().ToListAsync();
        var rooms = await context.Rooms.AsNoTracking().ToListAsync();
        var schedules = await context.StudySchedules.AsNoTracking().ToListAsync();
        var allGroups = await context.Groups.AsNoTracking().ToListAsync();
        var allTeachers = await context.Teachers.AsNoTracking().ToListAsync();

        var details = campuses.Select(c =>
        {
            var campusRoomCount = rooms.Count(r => r.CampusId == c.Id);
            var campusSchedules = schedules.Where(ss => ss.CampusId == c.Id).ToList();
            var campusGroupIds = campusSchedules.Select(ss => ss.GroupId).Distinct().ToList();
            var groupCount = campusGroupIds.Count;
            var campusStudents = context.Students.AsNoTracking().Count(s =>
                s.GroupId != null && campusGroupIds.Contains(s.GroupId.Value));
            var teacherCount = campusGroupIds
                .Select(gid => allGroups.FirstOrDefault(g => g.Id == gid)?.TeacherId)
                .Where(tid => tid.HasValue).Distinct().Count();

            return new CampusDetail
            {
                Id = c.Id,
                Name = c.Name,
                RoomCount = campusRoomCount,
                GroupCount = groupCount,
                StudentCount = campusStudents,
                TeacherCount = teacherCount
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

        var schedules = await context.StudySchedules.AsNoTracking().ToListAsync();

        var occupiedRoomIds = schedules.Select(ss => ss.RoomId).Distinct().ToHashSet();

        var schedulesByRoom = schedules
            .GroupBy(ss => ss.RoomId)
            .ToDictionary(g => g.Key, g => g.ToList());

        var groupIds = schedules.Select(ss => ss.GroupId).Distinct().ToList();
        var groupNames = await context.Groups.AsNoTracking()
            .Where(g => groupIds.Contains(g.Id))
            .ToListAsync();
        var groupNameMap = groupNames.ToDictionary(g => g.Id, g => g.Name);

        var details = rooms.Select(r =>
        {
            var isOccupied = occupiedRoomIds.Contains(r.Id);
            schedulesByRoom.TryGetValue(r.Id, out var roomSchedules);
            var firstSchedule = roomSchedules?.FirstOrDefault();
            groupNameMap.TryGetValue(firstSchedule?.GroupId ?? 0, out var groupName);
            var weekday = firstSchedule?.Weekday;
            var timeSlot = firstSchedule?.TimeSlot;
            return new RoomDetail
            {
                Id = r.Id,
                Name = r.Name,
                CampusName = r.Campus?.Name ?? string.Empty,
                IsOccupied = isOccupied,
                GroupName = groupName,
                Weekday = weekday,
                TimeSlot = timeSlot
            };
        }).ToList();

        return new RoomStatisticsResponse
        {
            TotalRooms = rooms.Count,
            OccupiedRooms = occupiedRoomIds.Count,
            EmptyRooms = rooms.Count - occupiedRoomIds.Count,
            Rooms = details
        };
    }

    public async Task<AttendanceStatisticsResponse> GetAttendanceStatisticsAsync(
        string? period, DateOnly? dateFrom, DateOnly? dateTo,
        int? month, int? year, string? gender)
    {
        var now = DateOnly.FromDateTime(DateTime.UtcNow);

        DateOnly start;
        DateOnly end;

        if (month.HasValue && year.HasValue)
        {
            start = new DateOnly(year.Value, month.Value, 1);
            end = start.AddMonths(1).AddDays(-1);
        }
        else if (dateFrom.HasValue || dateTo.HasValue)
        {
            start = dateFrom ?? now.AddDays(-30);
            end = dateTo ?? now;
        }
        else
        {
            start = period switch
            {
                "today" => now,
                "week" => now.AddDays(-(int)now.DayOfWeek),
                "month" => new DateOnly(now.Year, now.Month, 1),
                _ => now.AddDays(-7)
            };
            end = now;
        }

        var evaluations = await context.DailyEvaluations
            .AsNoTracking()
            .Where(e => e.SessionDate >= start && e.SessionDate <= end)
            .ToListAsync();

        var studentIds = evaluations.Select(e => e.StudentId).Distinct().ToList();
        var students = await context.Students.AsNoTracking()
            .Include(s => s.Group)
            .Where(s => studentIds.Contains(s.Id))
            .ToListAsync();
        var studentMap = students.ToDictionary(s => s.Id, s => s);

        if (!string.IsNullOrEmpty(gender))
        {
            var genderIsFemale = gender == "female";
            evaluations = evaluations.Where(e =>
                studentMap.TryGetValue(e.StudentId, out var stu) &&
                stu.IsFemale == genderIsFemale).ToList();
        }

        var present = 0;
        var absent = 0;
        var byGroupDict = new Dictionary<int, (int present, int absent)>();

        foreach (var e in evaluations)
        {
            if (e.Attendance == "present") present++;
            else if (e.Attendance == "absent") absent++;

            if (studentMap.TryGetValue(e.StudentId, out var stu) && stu.GroupId.HasValue)
            {
                var gid = stu.GroupId.Value;
                var current = byGroupDict.GetValueOrDefault(gid);
                if (e.Attendance == "present")
                    byGroupDict[gid] = (current.present + 1, current.absent);
                else if (e.Attendance == "absent")
                    byGroupDict[gid] = (current.present, current.absent + 1);
            }
        }

        var total = present + absent;
        var groupIds = byGroupDict.Keys.ToList();
        var groupNames = await context.Groups.AsNoTracking()
            .Where(g => groupIds.Contains(g.Id))
            .ToListAsync();
        var groupNameMap = groupNames.ToDictionary(g => g.Id, g => g.Name);

        var byGroup = byGroupDict.Select(kv =>
        {
            var t = kv.Value.present + kv.Value.absent;
            groupNameMap.TryGetValue(kv.Key, out var name);
            return new GroupAttendanceDetail
            {
                GroupId = kv.Key,
                GroupName = name ?? string.Empty,
                Present = kv.Value.present,
                Absent = kv.Value.absent,
                AttendanceRate = t > 0 ? Math.Round((double)kv.Value.present / t * 100, 1) : 0
            };
        }).OrderByDescending(d => d.AttendanceRate).ToList();

        return new AttendanceStatisticsResponse
        {
            TotalPresent = present,
            TotalAbsent = absent,
            AttendanceRate = total > 0 ? Math.Round((double)present / total * 100, 1) : 0,
            AbsenceRate = total > 0 ? Math.Round((double)absent / total * 100, 1) : 0,
            ByGroup = byGroup
        };
    }

    public async Task<AcademicStatisticsResponse> GetAcademicStatisticsAsync(
        DateOnly? dateFrom, DateOnly? dateTo,
        int? month, int? year, string? gender)
    {
        var evaluations = await context.DailyEvaluations.AsNoTracking().ToListAsync();

        if (month.HasValue && year.HasValue)
        {
            var mStart = new DateOnly(year.Value, month.Value, 1);
            var mEnd = mStart.AddMonths(1).AddDays(-1);
            evaluations = evaluations.Where(e => e.SessionDate >= mStart && e.SessionDate <= mEnd).ToList();
        }
        else if (dateFrom.HasValue || dateTo.HasValue)
        {
            var dFrom = dateFrom ?? new DateOnly(2020, 1, 1);
            var dTo = dateTo ?? DateOnly.FromDateTime(DateTime.UtcNow);
            evaluations = evaluations.Where(e => e.SessionDate >= dFrom && e.SessionDate <= dTo).ToList();
        }

        var evalStudentIds = evaluations.Select(e => e.StudentId).Distinct().ToList();
        var evalStudents = await context.Students.AsNoTracking()
            .Include(s => s.Group)
            .Where(s => evalStudentIds.Contains(s.Id))
            .ToListAsync();
        var evalStudentMap = evalStudents.ToDictionary(s => s.Id, s => s);

        if (!string.IsNullOrEmpty(gender))
        {
            var genderIsFemale = gender == "female";
            evaluations = evaluations.Where(e =>
                evalStudentMap.TryGetValue(e.StudentId, out var stu) &&
                stu.IsFemale == genderIsFemale).ToList();
        }

        var evalValues = evaluations.Where(e => e.Evaluation.HasValue).Select(e => (double)e.Evaluation!.Value).ToList();
        var avgEvaluation = evalValues.Any() ? Math.Round(evalValues.Average(), 2) : 0;

        var memValues = evaluations.Where(e => !string.IsNullOrEmpty(e.NewMemorization) && double.TryParse(e.NewMemorization, out _))
            .Select(e => double.Parse(e.NewMemorization!)).ToList();
        var avgMemorization = memValues.Any() ? Math.Round(memValues.Average(), 2) : 0;

        var revValues = evaluations.Where(e => !string.IsNullOrEmpty(e.ReviewQuantity) && double.TryParse(e.ReviewQuantity, out _))
            .Select(e => double.Parse(e.ReviewQuantity!)).ToList();
        var avgReview = revValues.Any() ? Math.Round(revValues.Average(), 2) : 0;

        var lowEvalStudents = evaluations.Where(e => e.Evaluation.HasValue && e.Evaluation < 10)
            .Select(e => e.StudentId).Distinct().Count();

        var examResults = await context.ExamResults.AsNoTracking().ToListAsync();
        var grades = examResults.Where(r => r.FinalGrade.HasValue).Select(r => (double)r.FinalGrade!.Value).ToList();
        var avgExam = grades.Any() ? Math.Round(grades.Average(), 2) : 0;
        var lowExamStudents = examResults.Where(r => r.FinalGrade.HasValue && r.FinalGrade < 10)
            .Select(r => r.StudentId).Distinct().Count();

        var byGroupDict = new Dictionary<int, List<double>>();
        var byGroupMem = new Dictionary<int, List<double>>();
        var byGroupRev = new Dictionary<int, List<double>>();

        foreach (var e in evaluations)
        {
            if (!evalStudentMap.TryGetValue(e.StudentId, out var stu) || !stu.GroupId.HasValue) continue;
            var gid = stu.GroupId.Value;

            if (e.Evaluation.HasValue)
            {
                byGroupDict.TryAdd(gid, new List<double>());
                byGroupDict[gid].Add((double)e.Evaluation!.Value);
            }
            if (!string.IsNullOrEmpty(e.NewMemorization) && double.TryParse(e.NewMemorization, out var mv))
            {
                byGroupMem.TryAdd(gid, new List<double>());
                byGroupMem[gid].Add(mv);
            }
            if (!string.IsNullOrEmpty(e.ReviewQuantity) && double.TryParse(e.ReviewQuantity, out var rv))
            {
                byGroupRev.TryAdd(gid, new List<double>());
                byGroupRev[gid].Add(rv);
            }
        }

        var allGroupIds = byGroupDict.Keys.Union(byGroupMem.Keys).Union(byGroupRev.Keys).Distinct().ToList();
        var groupNames = await context.Groups.AsNoTracking()
            .Where(g => allGroupIds.Contains(g.Id))
            .ToListAsync();
        var groupNameMap = groupNames.ToDictionary(g => g.Id, g => g.Name);

        var byGroup = allGroupIds.Select(gid =>
        {
            groupNameMap.TryGetValue(gid, out var name);
            byGroupDict.TryGetValue(gid, out var ev);
            byGroupMem.TryGetValue(gid, out var mem);
            byGroupRev.TryGetValue(gid, out var rev);
            return new GroupAcademicDetail
            {
                GroupId = gid,
                GroupName = name ?? string.Empty,
                AvgEvaluation = ev != null && ev.Any() ? Math.Round(ev.Average(), 2) : 0,
                AvgMemorization = mem != null && mem.Any() ? Math.Round(mem.Average(), 2) : 0,
                AvgReview = rev != null && rev.Any() ? Math.Round(rev.Average(), 2) : 0
            };
        }).ToList();

        return new AcademicStatisticsResponse
        {
            AvgEvaluation = avgEvaluation,
            AvgMemorization = avgMemorization,
            AvgReview = avgReview,
            StudentsNeedingFollowUp = lowEvalStudents + lowExamStudents,
            AvgExamResult = avgExam,
            ByGroup = byGroup
        };
    }

    public async Task<ExamStatisticsResponse> GetExamStatisticsAsync(
        int? semesterId, int? groupId, string? gender)
    {
        var query = context.Exams.AsNoTracking().AsQueryable();

        if (semesterId.HasValue)
            query = query.Where(e => e.ExamPlan.SemesterId == semesterId.Value);
        if (groupId.HasValue)
            query = query.Where(e => e.GroupId == groupId.Value);
        if (!string.IsNullOrEmpty(gender))
        {
            var groups = await context.Groups.AsNoTracking().ToListAsync();
            var groupIds = gender == "male"
                ? groups.Where(g => !g.IsFemale).Select(g => g.Id).ToList()
                : groups.Where(g => g.IsFemale).Select(g => g.Id).ToList();
            query = query.Where(e => groupIds.Contains(e.GroupId));
        }

        var exams = await query.ToListAsync();
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        var completed = exams.Where(e => e.ExamDate <= today).ToList();
        var upcoming = exams.Where(e => e.ExamDate > today).ToList();

        var examIds = completed.Select(e => e.Id).ToList();
        var allResults = await context.ExamResults.AsNoTracking()
            .Where(r => examIds.Contains(r.ExamId))
            .ToListAsync();

        var grades = allResults.Where(r => r.FinalGrade.HasValue)
            .Select(r => (double)r.FinalGrade!.Value).ToList();

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
