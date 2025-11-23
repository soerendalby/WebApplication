using System;

namespace WebApplication.Models;

public enum ApprovalDecision
{
    Approved,
    Rejected
}

public class Approval
{
    public Guid Id { get; set; }
    public Guid RequestId { get; set; }
    public Guid ApproverId { get; set; }
    public ApprovalDecision Decision { get; set; }
    public string? Comment { get; set; }
    public DateTime DecidedAt { get; set; } = DateTime.UtcNow;

    public Request? Request { get; set; }
    public Approver? Approver { get; set; }
}