using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebApplication.Data;
using WebApplication.Models;

namespace WebApplication.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly AppDbContext _db;

        public IndexModel(ILogger<IndexModel> logger, AppDbContext db)
        {
            _logger = logger;
            _db = db;
        }

        public int StudentActiveRequests { get; set; }
        public int ApproverPendingCount { get; set; }
        public int AdminProgramCount { get; set; }

        public async Task OnGetAsync()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                var role = User.Claims.FirstOrDefault(c => c.Type == "role")?.Value;
                var email = User.Identity.Name;

                if (role == "student" && email != null)
                {
                    var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == email);
                    if (user != null)
                    {
                        StudentActiveRequests = await _db.Requests.CountAsync(r => r.UserId == user.Id && r.Status == RequestStatus.Pending);
                    }
                }
                else if (role == "approver" && email != null)
                {
                    var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == email);
                    if (user != null)
                    {
                        var programIds = await _db.Approvers
                            .Where(a => a.UserId == user.Id && a.Active)
                            .Select(a => a.StudyProgramId)
                            .ToListAsync();

                        ApproverPendingCount = await _db.Requests
                            .CountAsync(r => r.Status == RequestStatus.Pending && programIds.Contains(r.StudyProgramId));
                    }
                }
                else if (role == "admin")
                {
                    AdminProgramCount = await _db.StudyPrograms.CountAsync();
                }
            }
        }
    }
}
