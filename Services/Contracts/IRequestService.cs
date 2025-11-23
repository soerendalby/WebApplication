using WebApplication.Models;

namespace WebApplication.Services.Contracts;

public interface IRequestService
{
    Task<Request> CreateAsync(Guid studentId, Guid studyProgramId, bool hasValidLicense, bool readGuidelines, CancellationToken ct = default);
}
