using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebApplication.Data;
using WebApplication.Models;

namespace WebApplication.Pages.Approvals;

public class IndexModel : PageModel
{
    private readonly AppDbContext _db;
    public List<Request> Pending { get; set; } = new();

    public IndexModel(AppDbContext db)
    {
        _db = db;
    }

    public async Task OnGet()
    {
        var email = User.Identity?.Name ?? string.Empty;
        var approverUserId = await _db.Users.Where(u => u.Email == email).Select(u => u.Id).FirstOrDefaultAsync();
        var programIds = await _db.Approvers.Where(a => a.UserId == approverUserId && a.Active).Select(a => a.StudyProgramId).ToListAsync();

        Pending = await _db.Requests
            .Include(r => r.User)
            .Include(r => r.StudyProgram)
            .Where(r => r.Status == RequestStatus.Pending && programIds.Contains(r.StudyProgramId))
            .OrderBy(r => r.SubmittedAt)
            .ToListAsync();
    }

    public async Task<IActionResult> OnPostApproveAsync(Guid id, string? comment)
    {
        await DecideAsync(id, ApprovalDecision.Approved, comment);
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostRejectAsync(Guid id, string comment)
    {
        if (string.IsNullOrWhiteSpace(comment))
        {
            TempData["Alert"] = "Comment is required for rejection.";
            return RedirectToPage();
        }
        await DecideAsync(id, ApprovalDecision.Rejected, comment);
        return RedirectToPage();
    }

    private async Task DecideAsync(Guid requestId, ApprovalDecision decision, string? comment)
    {
        var email = User.Identity?.Name ?? string.Empty;
        var approverUser = await _db.Users.FirstAsync(u => u.Email == email);
        var approver = await _db.Approvers.FirstOrDefaultAsync(a => a.UserId == approverUser.Id)
                       ?? throw new InvalidOperationException("Not an approver for any program.");

        var req = await _db.Requests.FirstOrDefaultAsync(r => r.Id == requestId)
                  ?? throw new InvalidOperationException("Request not found.");

        // Enforce first decision wins
        var existing = await _db.Approvals.FirstOrDefaultAsync(a => a.RequestId == requestId);
        if (existing != null) return;

        _db.Approvals.Add(new Approval
        {
            Id = Guid.NewGuid(),
            RequestId = requestId,
            ApproverId = approver.Id,
            Decision = decision,
            Comment = string.IsNullOrWhiteSpace(comment) ? null : comment,
            DecidedAt = DateTime.UtcNow
        });

        req.Status = decision == ApprovalDecision.Approved ? RequestStatus.Approved : RequestStatus.Rejected;

        _db.AuditLogs.Add(new AuditLog
        {
            Id = Guid.NewGuid(),
            Timestamp = DateTime.UtcNow,
            UserId = approverUser.Id,
            UserEmail = approverUser.Email,
            Role = "approver",
            Action = decision == ApprovalDecision.Approved ? "approval.approve" : "approval.reject",
            RequestId = req.Id,
            StudyProgramId = req.StudyProgramId,
            Details = string.IsNullOrWhiteSpace(comment) ? null : comment
        });

        await _db.SaveChangesAsync();
    }
}
