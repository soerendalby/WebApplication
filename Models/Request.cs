using System;

namespace WebApplication.Models;

public enum RequestStatus
{
    Pending,
    Approved,
    Rejected,
    Expired
}

public class Request
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid StudyProgramId { get; set; }
    public RequestStatus Status { get; set; } = RequestStatus.Pending;
    public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ExpiresAt { get; set; }
    public DateTime? ApprovalRetractedAt { get; set; }

    public User? User { get; set; }
    public StudyProgram? StudyProgram { get; set; }
}