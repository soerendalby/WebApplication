using WebApplication.Data;
using WebApplication.Models;
using WebApplication.Services.Contracts;

namespace WebApplication.Services;

public class ApprovalService : IApprovalService
{
    private readonly AppDbContext _db;
    private readonly ILogger<ApprovalService> _logger;

    public ApprovalService(AppDbContext db, ILogger<ApprovalService> logger)
    {
        _db = db;
        _logger = logger;
    }

    public Task<Approval> ApproveAsync(Guid requestId, Guid approverId, string? comment, CancellationToken ct = default)
        => throw new NotImplementedException();

    public Task<Approval> RejectAsync(Guid requestId, Guid approverId, string comment, CancellationToken ct = default)
        => throw new NotImplementedException();

    public Task RetractAsync(Guid requestId, Guid approverId, CancellationToken ct = default)
        => throw new NotImplementedException();
}
