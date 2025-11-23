using WebApplication.Models;

namespace WebApplication.Services.Contracts;

public interface IProgramService
{
    Task<StudyProgram> CreateProgramAsync(string name, string? description, int? reminderDays, int? expiryDays, CancellationToken ct = default);
}
