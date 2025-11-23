using System;

namespace WebApplication.Models;

public class StudyProgram
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? ReminderDays { get; set; } = 8;
    public int? ExpiryDays { get; set; } = 10;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Approver> Approvers { get; set; } = new List<Approver>();
}