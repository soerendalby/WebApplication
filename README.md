# WebApplication

Small ASP.NET Core Razor Pages sample app targeting .NET 9.0.

This repository is intentionally minimal — it shows a Razor Pages app with static assets served from `wwwroot` and a small layout under `Pages/Shared/_Layout.cshtml`.

Prerequisites
- .NET 9 SDK installed (use the latest patch for .NET 9)

Quick start

```powershell
dotnet restore
dotnet build
dotnet run --project WebApplication.csproj
# for fast feedback
dotnet watch run
```

Project layout highlights
- `Program.cs` — app entry and service registration (calls `builder.Services.AddRazorPages()` and maps endpoints).
- `Pages/` — Razor Pages (.cshtml + PageModel .cshtml.cs). Use `asp-page` tag helpers for links.
- `Pages/Shared/_Layout.cshtml` — single layout file; references bootstrap and jQuery from `wwwroot/lib`.
- `wwwroot/` — static assets (put vendor libs under `wwwroot/lib`, app JS/CSS under `wwwroot/js` and `wwwroot/css`).

Notes for contributors
- The project uses Razor Pages (not MVC controllers). Keep UI code in `.cshtml` and page logic in PageModel classes.
- Static assets are versioned with `asp-append-version` in the layout; `Program.cs` calls `MapStaticAssets()` and `WithStaticAssets()` to enable the mapping used by the template.
- There is no DbContext or tests in the repo. If you add EF Core / SQLite, register the DbContext in `Program.cs` and add connection strings to `appsettings.json`.

How to add a Razor Page (summary)
1. Create `Pages/YourPage.cshtml` with `@page` and UI markup.
2. Create `Pages/YourPage.cshtml.cs` PageModel and an `OnGet`/`OnPost` handler as needed.
3. Link to it with `<a asp-page="/YourPage">YourPage</a>`.

CI and contribution
- No CI configured by default. A minimal GitHub Actions workflow should restore and build on push/pull_request.
- Branch naming: `feature/<desc>`, `fix/<desc>`; write concise commit messages starting with a verb.

If anything above is out of date for your environment (SDK versions, CI policies, or deployment targets), update this README and `.github/copilot-instructions.md` accordingly.
