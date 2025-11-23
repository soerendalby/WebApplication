using WebApplication.Models;

namespace WebApplication.Services.Contracts;

public interface IApprovalService
{
    Task<Approval> ApproveAsync(Guid requestId, Guid approverId, string? comment, CancellationToken ct = default);
    Task<Approval> RejectAsync(Guid requestId, Guid approverId, string comment, CancellationToken ct = default);
    Task RetractAsync(Guid requestId, Guid approverId, CancellationToken ct = default);
}
