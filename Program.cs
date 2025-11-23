using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using WebApplication.Services;
using WebApplication.Services.Contracts;
namespace WebApplication
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = Microsoft.AspNetCore.Builder.WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorPages();

            // Data: EF Core + SQLite
            builder.Services.AddDbContext<WebApplication.Data.AppDbContext>(options =>
                options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

            // AuthN/Z: Cookie auth with simple role policies (demo)
            builder.Services
                .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/Auth/Login";
                    options.AccessDeniedPath = "/";
                });

            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("Student", p => p.RequireClaim("role", "student"));
                options.AddPolicy("Approver", p => p.RequireClaim("role", "approver"));
                options.AddPolicy("Admin", p => p.RequireClaim("role", "admin"));
            });

            // App services
            builder.Services.AddScoped<IRequestService, RequestService>();
            builder.Services.AddScoped<IApprovalService, ApprovalService>();
            builder.Services.AddScoped<IAuditService, AuditService>();
            // builder.Services.AddHostedService<WebApplication.Services.Hosted.ReminderWorker>(); // enable later when logic is added

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            // Ensure database is created (MVP)
            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<WebApplication.Data.AppDbContext>();
                db.Database.EnsureCreated();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapRazorPages()
               .WithStaticAssets();

            app.Run();
        }
    }
}
