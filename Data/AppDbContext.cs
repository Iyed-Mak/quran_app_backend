using Microsoft.EntityFrameworkCore;
using QuranSchool.Api.Models;

namespace QuranSchool.Api.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Admin> Admins => Set<Admin>();
    public DbSet<Teacher> Teachers => Set<Teacher>();
    public DbSet<Parent> Parents => Set<Parent>();
    public DbSet<AcademicYear> AcademicYears => Set<AcademicYear>();
    public DbSet<Campus> Campuses => Set<Campus>();
    public DbSet<RequiredDocument> RequiredDocuments => Set<RequiredDocument>();
    public DbSet<Group> Groups => Set<Group>();
    public DbSet<Room> Rooms => Set<Room>();
    public DbSet<Semester> Semesters => Set<Semester>();
    public DbSet<Student> Students => Set<Student>();
    public DbSet<ExamPlan> ExamPlans => Set<ExamPlan>();
    public DbSet<Exam> Exams => Set<Exam>();
    public DbSet<ExamResult> ExamResults => Set<ExamResult>();
    public DbSet<DailyEvaluation> DailyEvaluations => Set<DailyEvaluation>();
    public DbSet<Homework> Homeworks => Set<Homework>();
    public DbSet<TeacherAttendance> TeacherAttendances => Set<TeacherAttendance>();
    public DbSet<StudySchedule> StudySchedules => Set<StudySchedule>();
    public DbSet<StudentDocument> StudentDocuments => Set<StudentDocument>();
    public DbSet<AppNotification> Notifications => Set<AppNotification>();
    public DbSet<NotificationReceiver> NotificationReceivers => Set<NotificationReceiver>();
    public DbSet<RevokedToken> RevokedTokens => Set<RevokedToken>();
    public DbSet<PasswordResetToken> PasswordResetTokens => Set<PasswordResetToken>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ExamResult>()
            .Property(e => e.Status)
            .HasConversion<string>();

        modelBuilder.Entity<ExamResult>()
            .Property(e => e.ExamGrade).HasPrecision(5, 2);
        modelBuilder.Entity<ExamResult>()
            .Property(e => e.ContinuousEvaluation).HasPrecision(5, 2);
        modelBuilder.Entity<ExamResult>()
            .Property(e => e.FinalGrade).HasPrecision(5, 2);

        modelBuilder.Entity<DailyEvaluation>()
            .Property(e => e.Evaluation).HasPrecision(5, 2);

        modelBuilder.Entity<Admin>()
            .HasIndex(a => a.Username).IsUnique();
        modelBuilder.Entity<Teacher>()
            .HasIndex(t => t.Username).IsUnique();
        modelBuilder.Entity<Parent>()
            .HasIndex(p => p.Username).IsUnique();
        modelBuilder.Entity<Student>()
            .HasIndex(s => s.Username).IsUnique();
        modelBuilder.Entity<Student>()
            .HasIndex(s => s.SerialNumber).IsUnique();

        modelBuilder.Entity<RevokedToken>()
            .HasIndex(r => r.JwtId).IsUnique();
        modelBuilder.Entity<PasswordResetToken>()
            .HasIndex(p => p.Token).IsUnique();

        modelBuilder.Entity<ExamPlan>()
            .HasOne(e => e.Creator)
            .WithMany(a => a.ExamPlans)
            .HasForeignKey(e => e.CreatedBy)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Group>()
            .HasOne(g => g.Teacher)
            .WithMany(t => t.Groups)
            .HasForeignKey(g => g.TeacherId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Student>()
            .HasOne(s => s.Parent)
            .WithMany(p => p.Students)
            .HasForeignKey(s => s.ParentId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Student>()
            .HasOne(s => s.Group)
            .WithMany(g => g.Students)
            .HasForeignKey(s => s.GroupId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Exam>()
            .HasOne(e => e.Group)
            .WithMany(g => g.Exams)
            .HasForeignKey(e => e.GroupId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Exam>()
            .HasOne(e => e.Teacher)
            .WithMany(t => t.Exams)
            .HasForeignKey(e => e.TeacherId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Exam>()
            .HasOne(e => e.Campus)
            .WithMany(c => c.Exams)
            .HasForeignKey(e => e.CampusId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Exam>()
            .HasOne(e => e.Room)
            .WithMany(r => r.Exams)
            .HasForeignKey(e => e.RoomId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<StudySchedule>()
            .HasOne(s => s.Campus)
            .WithMany(c => c.StudySchedules)
            .HasForeignKey(s => s.CampusId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<StudySchedule>()
            .HasOne(s => s.Room)
            .WithMany(r => r.StudySchedules)
            .HasForeignKey(s => s.RoomId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<StudySchedule>()
            .HasOne(s => s.Group)
            .WithMany(g => g.StudySchedules)
            .HasForeignKey(s => s.GroupId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<TeacherAttendance>()
            .HasOne(t => t.Group)
            .WithMany(g => g.TeacherAttendances)
            .HasForeignKey(t => t.GroupId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<TeacherAttendance>()
            .HasOne(t => t.Teacher)
            .WithMany(t => t.TeacherAttendances)
            .HasForeignKey(t => t.TeacherId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<DailyEvaluation>()
            .HasOne(e => e.Teacher)
            .WithMany(t => t.DailyEvaluations)
            .HasForeignKey(e => e.TeacherId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Homework>()
            .HasOne(h => h.Teacher)
            .WithMany(t => t.Homeworks)
            .HasForeignKey(h => h.TeacherId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<StudentDocument>()
            .HasOne(d => d.RequiredDocument)
            .WithMany(r => r.StudentDocuments)
            .HasForeignKey(d => d.RequiredDocumentId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<StudentDocument>()
            .HasOne(d => d.Student)
            .WithMany(s => s.StudentDocuments)
            .HasForeignKey(d => d.StudentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
