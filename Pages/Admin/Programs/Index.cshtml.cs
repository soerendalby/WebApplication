using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebApplication.Data;
using WebApplication.Models;

namespace WebApplication.Pages.Admin.Programs;

public class IndexModel : PageModel
{
    private readonly AppDbContext _db;
    public List<StudyProgram> Programs { get; set; } = new();

    public IndexModel(AppDbContext db)
    {
        _db = db;
    }

    public async Task OnGet()
    {
        Programs = await _db.StudyPrograms
            .Include(p => p.Approvers)
            .ThenInclude(a => a.User)
            .OrderBy(p => p.Name)
            .ToListAsync();
    }

    public async Task<IActionResult> OnPostCreateAsync(string name, string? description, int? reminderDays, int? expiryDays)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            TempData["Alert"] = "Name is required.";
            return RedirectToPage();
        }
        _db.StudyPrograms.Add(new StudyProgram
        {
            Id = Guid.NewGuid(),
            Name = name.Trim(),
            Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim(),
            ReminderDays = reminderDays,
            ExpiryDays = expiryDays
        });
        await _db.SaveChangesAsync();
        TempData["Alert"] = "Program created.";
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostAddApproverAsync(Guid programId, string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            TempData["Alert"] = "Email is required.";
            return RedirectToPage();
        }
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (user == null)
        {
            user = new User { Id = Guid.NewGuid(), Email = email.Trim(), Role = "approver" };
            _db.Users.Add(user);
        }
        else if (!string.Equals(user.Role, "approver", StringComparison.OrdinalIgnoreCase))
        {
            user.Role = "approver";
        }
        var exists = await _db.Approvers.AnyAsync(a => a.UserId == user.Id && a.StudyProgramId == programId);
        if (!exists)
        {
            _db.Approvers.Add(new Approver { Id = Guid.NewGuid(), UserId = user.Id, StudyProgramId = programId, Active = true });
        }
        await _db.SaveChangesAsync();
        TempData["Alert"] = "Approver assigned.";
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostRemoveApproverAsync(Guid approverId)
    {
        var approver = await _db.Approvers.FirstOrDefaultAsync(a => a.Id == approverId);
        if (approver != null)
        {
            _db.Approvers.Remove(approver);
            await _db.SaveChangesAsync();
        }
        TempData["Alert"] = "Approver removed.";
        return RedirectToPage();
    }
}
