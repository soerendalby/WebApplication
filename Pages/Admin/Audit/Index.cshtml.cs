using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebApplication.Data;
using WebApplication.Models;
using WebApplication.Services.Util;

namespace WebApplication.Pages.Admin.Audit;

public class IndexModel : PageModel
{
    private readonly AppDbContext _db;

    public IndexModel(AppDbContext db)
    {
        _db = db;
    }

    [BindProperty(SupportsGet = true)]
    [DataType(DataType.Date)]
    public DateTime? From { get; set; }

    [BindProperty(SupportsGet = true)]
    [DataType(DataType.Date)]
    public DateTime? To { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? ActionContains { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? Role { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? EmailContains { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? RequestId { get; set; }

    public List<AuditLog> Logs { get; set; } = new();

    public async Task OnGetAsync(CancellationToken ct)
    {
        Logs = await Query().OrderByDescending(a => a.Timestamp).Take(500).ToListAsync(ct);
    }

    public async Task<IActionResult> OnGetExportAsync(CancellationToken ct)
    {
        var rows = await Query().OrderBy(a => a.Timestamp).ToListAsync(ct);
        var headers = new[] { "id", "timestamp", "user_id", "user_email", "role", "action", "request_id", "study_program_id", "details" };
        byte[] csv = CsvWriter.WriteWithHeader(rows, headers, r => new[]
        {
            r.Id.ToString(),
            r.Timestamp.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ"),
            r.UserId.ToString(),
            r.UserEmail,
            r.Role,
            r.Action,
            r.RequestId?.ToString() ?? string.Empty,
            r.StudyProgramId?.ToString() ?? string.Empty,
            r.Details ?? string.Empty
        });
        var fileName = $"audit-{DateTime.UtcNow:yyyyMMdd-HHmmss}.csv";
        return File(csv, "text/csv; charset=utf-8", fileName);
    }

    private IQueryable<AuditLog> Query()
    {
        var q = _db.AuditLogs.AsQueryable();
        if (From.HasValue)
        {
            // Interpret From as start of day UTC for simplicity
            var fromUtc = DateTime.SpecifyKind(From.Value.Date, DateTimeKind.Utc);
            q = q.Where(a => a.Timestamp >= fromUtc);
        }
        if (To.HasValue)
        {
            // Inclusive end of day
            var toUtc = DateTime.SpecifyKind(To.Value.Date.AddDays(1).AddTicks(-1), DateTimeKind.Utc);
            q = q.Where(a => a.Timestamp <= toUtc);
        }
        if (!string.IsNullOrWhiteSpace(ActionContains))
        {
            q = q.Where(a => a.Action.Contains(ActionContains!));
        }
        if (!string.IsNullOrWhiteSpace(Role))
        {
            q = q.Where(a => a.Role == Role);
        }
        if (!string.IsNullOrWhiteSpace(EmailContains))
        {
            q = q.Where(a => a.UserEmail.Contains(EmailContains!));
        }
        if (!string.IsNullOrWhiteSpace(RequestId) && Guid.TryParse(RequestId, out var rid))
        {
            q = q.Where(a => a.RequestId == rid);
        }
        return q;
    }
}
