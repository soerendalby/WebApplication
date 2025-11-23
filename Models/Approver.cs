using System;

namespace WebApplication.Models;

public class Approver
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid StudyProgramId { get; set; }
    public bool Active { get; set; } = true;
    public string? ExtraScope { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public User? User { get; set; }
    public StudyProgram? StudyProgram { get; set; }
}