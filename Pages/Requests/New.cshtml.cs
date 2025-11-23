using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebApplication.Data;
using WebApplication.Models;
using WebApplication.Services.Contracts;
using WebApplication.Validation;

namespace WebApplication.Pages.Requests;

public class NewModel : PageModel
{
    private readonly AppDbContext _db;
    private readonly IRequestService _requests;

    public NewModel(AppDbContext db, IRequestService requests)
    {
        _db = db;
        _requests = requests;
    }

    public List<StudyProgram> Programs { get; set; } = new();

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public async Task OnGet()
    {
        Programs = await _db.StudyPrograms.OrderBy(p => p.Name).ToListAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        Programs = await _db.StudyPrograms.OrderBy(p => p.Name).ToListAsync();
        if (!ModelState.IsValid)
            return Page();

        var email = User.Identity?.Name ?? string.Empty;
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (user == null)
        {
            ModelState.AddModelError(string.Empty, "User not found.");
            return Page();
        }

        try
        {
            await _requests.CreateAsync(user.Id, Input.StudyProgramId, Input.HasValidLicense, Input.ReadGuidelines);
            TempData["Alert"] = "Request submitted.";
            return RedirectToPage("/Requests/Index");
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return Page();
        }
    }

    public class InputModel
    {
        [Required]
        [Display(Name = "Study program")]
        public Guid StudyProgramId { get; set; }

    [Display(Name = "I confirm I have a valid driver's license")]
    [MustBeTrue(ErrorMessage = "You must confirm you have a valid driver's license.")]
    public bool HasValidLicense { get; set; }

    [Display(Name = "I have read the guidelines")]
    [MustBeTrue(ErrorMessage = "You must confirm you have read the guidelines.")]
    public bool ReadGuidelines { get; set; }
    }
}
