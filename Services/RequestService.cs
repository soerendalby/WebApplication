using Microsoft.EntityFrameworkCore;
using WebApplication.Data;
using WebApplication.Models;
using WebApplication.Services.Contracts;

namespace WebApplication.Services;

public class RequestService : IRequestService
{
    private readonly AppDbContext _db;
    private readonly ILogger<RequestService> _logger;

    public RequestService(AppDbContext db, ILogger<RequestService> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<Request> CreateAsync(Guid studentId, Guid studyProgramId, bool hasValidLicense, bool readGuidelines, CancellationToken ct = default)
    {
        if (!hasValidLicense || !readGuidelines)
            throw new InvalidOperationException("Both confirmations are required.");

        var program = await _db.StudyPrograms.FirstOrDefaultAsync(p => p.Id == studyProgramId, ct)
                      ?? throw new InvalidOperationException("Study program not found.");

        var approverCount = await _db.Approvers.CountAsync(a => a.StudyProgramId == studyProgramId && a.Active, ct);
        var now = DateTime.UtcNow;
        var expiryDays = program.ExpiryDays ?? 10;
        var req = new Request
        {
            Id = Guid.NewGuid(),
            UserId = studentId,
            StudyProgramId = studyProgramId,
            Status = approverCount > 0 ? RequestStatus.Pending : RequestStatus.Approved,
            SubmittedAt = now,
            ExpiresAt = now.AddDays(expiryDays)
        };
        _db.Requests.Add(req);
        _db.AuditLogs.Add(new AuditLog
        {
            Id = Guid.NewGuid(),
            Timestamp = now,
            UserId = studentId,
            UserEmail = await _db.Users.Where(u => u.Id == studentId).Select(u => u.Email).FirstOrDefaultAsync(ct) ?? string.Empty,
            Role = "student",
            Action = "request.create",
            RequestId = req.Id,
            StudyProgramId = req.StudyProgramId,
            Details = approverCount > 0 ? null : "auto-approved"
        });
        await _db.SaveChangesAsync(ct);
        return req;
    }
}
