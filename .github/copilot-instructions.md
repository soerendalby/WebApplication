## Quick context

This is a small ASP.NET Core MVC (Controllers + Views) application targeting .NET 9.0.

- Project file: `WebApplication.csproj` (TargetFramework: `net9.0`, `Nullable` = enabled, `ImplicitUsings` enabled)
- App entry: `Program.cs` — minimal WebApplication builder using MVC
- UI: Controllers under `Controllers/` and Views under `Views/ControllerName/Action.cshtml`
- Layout and shared view bits: `Views/Shared/_Layout.cshtml`
- Static assets: `wwwroot/` (third-party libs under `wwwroot/lib`, custom `wwwroot/js`, `wwwroot/css`)

Use this file to guide automated agents (Copilot-style) so edits fit the project's structure and conventions.

## What to know up front

- Routing: This repo uses MVC conventional routing: `{controller=Home}/{action=Index}/{id?}`. Prefer tag helpers like `asp-controller` and `asp-action` over hard-coded URLs (example: `<a asp-controller="Home" asp-action="Privacy">`).
- Services: `builder.Services.AddControllersWithViews()` is the baseline. Add additional services (DbContexts, Auth, etc.) by editing `Program.cs` near the builder block.
- Static assets mapping: `Program.cs` calls `app.MapStaticAssets()` and maps MVC routes — static web assets and `wwwroot` are served and versioned using tag helpers like `asp-append-version`.
- Tag helpers & anchors: Keep UI-linking via Tag Helpers instead of hard-coded URLs where possible (see `Views/Shared/_Layout.cshtml`).

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

- Controllers & actions: Place controllers in `Controllers/` (e.g., `HomeController`). Views live in `Views/<Controller>/<Action>.cshtml`. Keep controller actions thin and move UI-only logic into views or view models.
- Static assets location: put vendor packages under `wwwroot/lib/` and project JS/CSS under `wwwroot/js` and `wwwroot/css`. Reference them in `_Layout.cshtml` using `~/` paths and `asp-append-version` when you want cache-busting.
- Layout: `Views/Shared/_Layout.cshtml` is the single layout. Update nav links there using `asp-controller`/`asp-action`, not hard-coded URLs.
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

- Minimal service registration and endpoint mapping: `Program.cs` (lines around `AddControllersWithViews` / `MapControllerRoute`)
- Layout & asset references: `Views/Shared/_Layout.cshtml` — reference `~/lib/bootstrap/dist/css/bootstrap.min.css`, `~/js/site.js`, and use `asp-append-version="true"` for static assets.
- Home page example: `Views/Home/Index.cshtml` rendered by `HomeController.Index()`.

## Helpful heuristics for edits

- Keep changes localized: UI changes belong in `Views/` or `wwwroot`; service wiring and route config go in `Program.cs` or small extension methods in `Extensions/`.
- Prefer Tag Helpers for links and asset versioning (see `_Layout.cshtml`): use `asp-controller`, `asp-action`, and `asp-route-*`.
- Prefer conventional routing unless you have a clear reason to add attribute routing on controllers.

## When you need to modify project structure

- Adding EF: update `WebApplication.csproj` to include EF packages, add `AppDbContext` under `Data/`, add connection strings to `appsettings.json`, and register the context in `Program.cs` near `AddControllersWithViews()`.
- Adding API controllers: create `Controllers/Api` (or `Areas/Api`) and register `builder.Services.AddControllers()`; map endpoints with `app.MapControllers()` alongside the MVC route.

## What I won't assume

- There are no tests, no DbContext, and no external service integration shown in the repo. Do not remove or change `MapStaticAssets` or static web asset versioning unless you understand how assets are served.

---

If any of this is missing or you have repository-specific rules I didn't detect (CI config, secret management, or database infra), tell me and I'll update this file. Ready to iterate on wording or add examples of code edits you'd like automated.

## Common tasks (copy-paste snippets)

### Add a new MVC Controller + View
To add a new `Products` controller with an `Index` view:

1) Create the controller: `Controllers/ProductsController.cs`

```csharp
using Microsoft.AspNetCore.Mvc;

public class ProductsController : Controller
{
  public IActionResult Index()
  {
    // load data for the view (e.g., from a service)
    return View();
  }
}
```

2) Create the view: `Views/Products/Index.cshtml`

```cshtml
@{
  ViewData["Title"] = "Products";
}

<h1>Products</h1>
<!-- page UI here -->
```

3) Link from the layout or other views using tag helpers:

```html
<a asp-controller="Products" asp-action="Index">Products</a>
```

If you prefer scaffolding, use Visual Studio to scaffold a controller with views using Entity Framework, or create the two files manually.

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

3) Register the context in `Program.cs` (near `builder.Services.AddControllersWithViews()`):

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
