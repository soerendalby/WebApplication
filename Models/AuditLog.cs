using System;

namespace WebApplication.Models;

public class AuditLog
{
    public Guid Id { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public Guid UserId { get; set; }
    public string UserEmail { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty; // student|approver|admin
    public string Action { get; set; } = string.Empty;
    public Guid? RequestId { get; set; }
    public Guid? StudyProgramId { get; set; }
    public string? Details { get; set; }
}