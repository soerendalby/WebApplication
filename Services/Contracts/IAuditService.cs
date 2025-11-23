using WebApplication.Models;

namespace WebApplication.Services.Contracts;

public interface IAuditService
{
    Task AppendAsync(AuditLog entry, CancellationToken ct = default);
}
