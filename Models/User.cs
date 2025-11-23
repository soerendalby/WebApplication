using System;

namespace WebApplication.Models;

public class User
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string? Name { get; set; }
    // Values: student | approver | admin (demo roles)
    public string Role { get; set; } = "student";
    public Guid? StudyProgramId { get; set; }
    public bool? Verified { get; set; }
    public string? Locale { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public StudyProgram? StudyProgram { get; set; }
}