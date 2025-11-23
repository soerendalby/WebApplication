using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApplication.Data;
using WebApplication.Models;

namespace WebApplication.Pages.Auth;

public class LoginModel : PageModel
{
    [BindProperty]
    public LoginInput Input { get; set; } = new();
    private readonly AppDbContext _db;

    public LoginModel(AppDbContext db)
    {
        _db = db;
    }

    public void OnGet() {}

    public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        if (Input.Password != "demo")
        {
            ModelState.AddModelError(string.Empty, "Invalid credentials.");
            return Page();
        }

        var role = GetRoleFromEmail(Input.Email);
        if (role is null)
        {
            ModelState.AddModelError(string.Empty, "Email domain not allowed.");
            return Page();
        }

        // Ensure a User record exists for this email/role (demo setup)
        var dbUser = _db.Users.FirstOrDefault(u => u.Email == Input.Email);
        if (dbUser == null)
        {
            dbUser = new User { Id = Guid.NewGuid(), Email = Input.Email, Role = role };
            _db.Users.Add(dbUser);
            await _db.SaveChangesAsync();
        }
        else if (!string.Equals(dbUser.Role, role, StringComparison.OrdinalIgnoreCase))
        {
            dbUser.Role = role;
            await _db.SaveChangesAsync();
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, Input.Email),
            new Claim("role", role)
        };
        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            return Redirect(returnUrl);

        return RedirectToPage("/Index");
    }

    private static string? GetRoleFromEmail(string email)
    {
        var lower = email.Trim().ToLowerInvariant();
        if (lower.EndsWith("@student.dk")) return "student";
        if (lower.EndsWith("@admin.dk")) return "approver";
        if (lower.EndsWith("@mb.dk")) return "admin";
        return null;
    }
}

public class LoginInput
{
    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}
