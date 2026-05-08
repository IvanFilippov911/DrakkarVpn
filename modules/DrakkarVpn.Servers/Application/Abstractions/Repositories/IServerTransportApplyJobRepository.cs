using DrakkarVpn.Servers.Infrastructure.EF.Entities;

namespace DrakkarVpn.Core.Api.Modules.Servers.Application.Abstractions;

public interface IServerTransportApplyJobRepository
{
    Task<Guid> CreateOrGetAsync(
        Guid serverId,
        Guid activationId,
        long targetTransportVersion,
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

    Task<int> MarkCompletedAsync(
        IReadOnlyCollection<Guid> jobIds,
        string leaseOwner,
        DateTime utcNow,
        CancellationToken ct);

    Task<int> RescheduleAsync(
        Guid jobId,
        string leaseOwner,
        int newAttempt,
        DateTime nextAttemptAtUtc,
        string code,
        string? message,
        DateTime utcNow,
        CancellationToken ct);

    Task<int> MarkFailedAsync(
        Guid jobId,
        string leaseOwner,
        string code,
        string? message,
        DateTime utcNow,
        CancellationToken ct);

    Task<int> MarkFailedBatchAsync(
        IReadOnlyList<ServerTransportApplyJobMarkFailedBatchRow> rows,
        string leaseOwner,
        DateTime utcNow,
        CancellationToken ct);

    Task<int> RescheduleBatchAsync(
        IReadOnlyList<ServerTransportApplyJobRescheduleBatchRow> rows,
        string leaseOwner,
        DateTime utcNow,
        CancellationToken ct);

    Task<int> MarkObsoleteAsync(
        IReadOnlyCollection<Guid> jobIds,
        string leaseOwner,
        DateTime utcNow,
        CancellationToken ct);
}