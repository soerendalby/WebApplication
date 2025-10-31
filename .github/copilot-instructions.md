## Quick context

This is a small ASP.NET Core Razor Pages application targeting .NET 9.0.

- Project file: `WebApplication.csproj` (TargetFramework: `net9.0`, `Nullable` = enabled, `ImplicitUsings` enabled)
- App entry: `Program.cs` — minimal WebApplication builder using Razor Pages
- UI: Razor Pages under `Pages/` (page files + PageModel classes in `.cshtml.cs`)
- Layout and shared view bits: `Pages/Shared/_Layout.cshtml`
- Static assets: `wwwroot/` (third-party libs under `wwwroot/lib`, custom `wwwroot/js`, `wwwroot/css`)

Use this file to guide automated agents (Copilot-style) so edits fit the project's structure and conventions.

## What to know up front

- Routing: This repo uses Razor Pages (not MVC controllers). Pages are referenced with `asp-page` tag helpers (example: `<a asp-page="/Privacy">`).
- Services: `builder.Services.AddRazorPages()` is the baseline. Add additional services (DbContexts, Auth, etc.) by editing `Program.cs` near the builder block.
- Static assets mapping: `Program.cs` calls `app.MapStaticAssets()` and `app.MapRazorPages().WithStaticAssets()` — static web assets and `wwwroot` are served and versioned using tag helpers like `asp-append-version`.
- Tag helpers & anchors: Keep UI-linking via Tag Helpers instead of hard-coded URLs where possible (see `Pages/Shared/_Layout.cshtml`).

## Build / run / debug (concrete)

- Restore & build (CLI):

  dotnet restore
  dotnet build

- Run (CLI):

  dotnet run --project WebApplication.csproj

- Fast local feedback (watch):

  dotnet watch run

- Debugging: open `WebApplication.sln` in Visual Studio (or VS Code with C# extension). The solution contains `Properties/launchSettings.json` used by the debugger / IIS Express profiles.

## Project-specific conventions & patterns

- Razor-page naming: Page files are `Pages/Name.cshtml` with code-behind `Pages/Name.cshtml.cs` (PageModel). Keep page-specific business logic in the PageModel and UI-only logic in the `.cshtml`.
- Static assets location: put vendor packages under `wwwroot/lib/` and project JS/CSS under `wwwroot/js` and `wwwroot/css`. Reference them in `_Layout.cshtml` using `~/` paths and `asp-append-version` when you want cache-busting.
- Layout: `Pages/Shared/_Layout.cshtml` is the single layout. Update nav links there (use `asp-page`), not in each page.
- No tests currently: there is no test project; if you add tests, name the project `WebApplication.Tests` and place it alongside the solution.

## Integration points and external deps

- Database: there is no DbContext or connection string in the repository. If adding EF Core + SQL Server, add `ConnectionStrings` to `appsettings.json` and register your context in `Program.cs`:

```csharp
// example (add when EF packages are added)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
```

Then use the EF Core CLI for migrations:

  dotnet tool install --global dotnet-ef
  dotnet ef migrations add InitialCreate --project WebApplication
  dotnet ef database update --project WebApplication

- Front-end libs: this project already includes Bootstrap and jQuery under `wwwroot/lib/`. Use the same pattern (add to `wwwroot/lib` and reference in `_Layout.cshtml`).

## Examples from the codebase (quick lookups)

- Minimal service registration and endpoint mapping: `Program.cs` (lines around AddRazorPages / MapRazorPages)
- Layout & asset references: `Pages/Shared/_Layout.cshtml` — shows `~/lib/bootstrap/dist/css/bootstrap.min.css`, `~/js/site.js`, and uses `asp-append-version="true"` for static assets.
- Home page example: `Pages/Index.cshtml` — very small page; PageModel logic is in `Pages/Index.cshtml.cs` when present.

## Helpful heuristics for edits

- Keep changes localized: UI changes belong in `Pages/` or `wwwroot`; service wiring goes in `Program.cs` or small extension methods in `Extensions/`.
- Prefer Tag Helpers for links and asset versioning (see `_Layout.cshtml`).
- Avoid adding controllers — add Razor Pages unless you intentionally switch to MVC patterns and update `WebApplication.csproj` accordingly.

## When you need to modify project structure

- Adding EF: update `WebApplication.csproj` to include EF packages, add `AppDbContext` under `Data/`, add connection strings to `appsettings.json`, and register the context in `Program.cs`.
- Adding API controllers: create an `Areas/Api` or `Controllers/` folder and register `builder.Services.AddControllers()` and map endpoints with `app.MapControllers()`.

## What I won't assume

- There are no tests, no DbContext, and no external service integration shown in the repo. Do not remove or change `MapStaticAssets` / `WithStaticAssets` unless you understand static web assets mapping.

---

If any of this is missing or you have repository-specific rules I didn't detect (CI config, secret management, or database infra), tell me and I'll update this file. Ready to iterate on wording or add examples of code edits you'd like automated.

## Common tasks (copy-paste snippets)

### Add a new Razor Page
Use Razor Pages (not controllers). To add a new page called `Products` with a PageModel:

1. Create the Razor view: `Pages/Products.cshtml`

```html
@page
@model ProductsModel
@{
    ViewData["Title"] = "Products";
}

<h1>Products</h1>
<!-- page UI here -->
```

2. Create the PageModel: `Pages/Products.cshtml.cs`

```csharp
using Microsoft.AspNetCore.Mvc.RazorPages;

public class ProductsModel : PageModel
{
    public void OnGet()
    {
        // load page data
    }
}
```

3. Link from the layout or other pages using tag helpers:

```html
<a asp-page="/Products">Products</a>
```

If you prefer the dotnet CLI for scaffolding, you can use Visual Studio scaffolding or create the two files manually — the framework discovers Razor Pages by file location.

### EF Core + SQL Server (copy-paste wiring)
This repo doesn't currently include EF Core. If you add EF Core + SQL Server, here are minimal, copy-paste snippets.

1) Add packages to the project (update `WebApplication.csproj` or run `dotnet add package`):

```xml
<ItemGroup>
  <PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="7.*" />
  <PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="7.*" />
</ItemGroup>
```

2) Create `Data/AppDbContext.cs`:

```csharp
using Microsoft.EntityFrameworkCore;

namespace WebApplication.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    // public DbSet<Product> Products { get; set; }
}
```

3) Register the context in `Program.cs` (near `builder.Services.AddRazorPages()`):

```csharp
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
```

4) Add a connection string to `appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=WebAppDb;Trusted_Connection=True;MultipleActiveResultSets=true"
}
```

5) EF Core CLI (one-time setup):

```powershell
# install if needed (global)
dotnet tool install --global dotnet-ef

# add a migration and update the database
dotnet ef migrations add InitialCreate --project WebApplication
dotnet ef database update --project WebApplication
```

Notes:
- Use appropriate package versions that match your target .NET version (the example shows `7.*` as a placeholder; upgrade to `8.*`/`9.*` as needed for .NET 9).
- Put entity classes under `Models/` or `Data/` depending on your preference.

### Commit / PR conventions and CI notes
If you have an existing team convention, prefer that. If not, these lightweight conventions keep PRs clear for reviewers and for automated agents:

- Branch naming: `feature/<short-desc>`, `fix/<short-desc>`, or `hotfix/<short-desc>`.
- Commit messages: a short title (50 chars) and optional body. Start with a verb: `Add Products page`.
- PR description: include what changed, why, and how to validate locally (build/run steps). Mention any required DB migrations.

CI suggestions (optional):
- This repo has no CI configuration currently. If you add CI, a minimal GitHub Actions workflow name `dotnet.yml` should:
  - run on push/pull_request
  - restore, build, and (optionally) run `dotnet ef migrations script` if you validate migrations
  - use `actions/setup-dotnet` and run `dotnet restore` / `dotnet build`

Example minimal step (conceptual):

```yaml
# .github/workflows/dotnet.yml (example)
name: .NET
on: [push, pull_request]
jobs:
  build:
    runs-on: windows-latest
    steps:
      - uses: actions/checkout@v4
      - uses: actions/setup-dotnet@v3
        with:
          dotnet-version: '9.0.x'
      - run: dotnet restore
      - run: dotnet build --configuration Release --no-restore
```

Keep CI simple: build first, add test steps only if you introduce tests.
