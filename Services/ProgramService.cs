using WebApplication.Data;
using WebApplication.Models;
using WebApplication.Services.Contracts;

namespace WebApplication.Services;

public class ProgramService : IProgramService
{
    private readonly AppDbContext _db;
    private readonly ILogger<ProgramService> _logger;

    public ProgramService(AppDbContext db, ILogger<ProgramService> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<StudyProgram> CreateProgramAsync(string name, string? description, int? reminderDays, int? expiryDays, CancellationToken ct = default)
    {
        var sp = new StudyProgram
        {
            Id = Guid.NewGuid(),
            Name = name,
            Description = description,
            ReminderDays = reminderDays,
            ExpiryDays = expiryDays,
            CreatedAt = DateTime.UtcNow
        };
        _db.StudyPrograms.Add(sp);
        await _db.SaveChangesAsync(ct);
        return sp;
    }
}
