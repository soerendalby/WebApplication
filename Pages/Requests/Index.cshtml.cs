using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebApplication.Data;
using WebApplication.Models;

namespace WebApplication.Pages.Requests;

public class IndexModel : PageModel
{
    private readonly AppDbContext _db;
    public List<Request> Requests { get; set; } = new();

    public IndexModel(AppDbContext db)
    {
        _db = db;
    }

    public async Task OnGet()
    {
        var email = User.Identity?.Name ?? string.Empty;
        var userId = await _db.Users.Where(u => u.Email == email).Select(u => u.Id).FirstOrDefaultAsync();
        Requests = await _db.Requests
            .Where(r => r.UserId == userId)
            .OrderByDescending(r => r.SubmittedAt)
            .ToListAsync();
    }
}
