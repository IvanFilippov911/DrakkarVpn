using DrakkarVpn.Servers.Infrastructure.EF.Entities;

namespace DrakkarVpn.Core.Api.Modules.Servers.Application.Abstractions;

public interface IServerTransportApplyJobRepository
{
    Task<Guid> CreateOrGetAsync(
        Guid serverId,
        Guid activationId,
        int maxAttempt,
        DateTime utcNow,
        CancellationToken ct);

    Task<Guid?> GetActiveJobIdByServerIdAsync(Guid serverId, CancellationToken ct);

    Task<ServerTransportApplyJob?> GetByIdAsync(Guid jobId, CancellationToken ct);

    Task<IReadOnlyList<ServerTransportApplyJob>> AcquireBatchAsync(
        int take,
        TimeSpan lease,
        DateTime utcNow,
        string leaseOwner,
        CancellationToken ct);

    Task MarkCompletedAsync(Guid jobId, DateTime utcNow, CancellationToken ct);

    Task RescheduleAsync(
        Guid jobId,
        int newAttempt,
        DateTime nextAttemptAtUtc,
        string code,
        string? message,
        DateTime utcNow,
        CancellationToken ct);

    Task MarkFailedAsync(
        Guid jobId,
        string code,
        string? message,
        DateTime utcNow,
        CancellationToken ct);
}