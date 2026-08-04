using System.ComponentModel.DataAnnotations.Schema;

namespace QuranSchool.Api.Models;

[Table("campus")]
public class Campus : IEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public string? Description { get; set; }

    public List<Room> Rooms { get; set; } = new();
    public List<Exam> Exams { get; set; } = new();
    public List<StudySchedule> StudySchedules { get; set; } = new();
}
