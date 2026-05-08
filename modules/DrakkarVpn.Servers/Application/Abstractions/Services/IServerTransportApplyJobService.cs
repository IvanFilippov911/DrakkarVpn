using DrakkarVpn.Servers.Application.DTOs.ServerTransportApplyJobs;

namespace DrakkarVpn.Servers.Application.Abstractions.Services;

public interface IServerTransportApplyJobService
{
    Task<Guid> EnqueueAsync(
        Guid serverId,
        Guid activationId,
        long targetTransportVersion,
        CancellationToken ct);

    Task<Guid?> GetActiveJobIdByServerIdAsync(Guid serverId, CancellationToken ct);

    Task<IReadOnlyList<ServerTransportApplyJobDto>> AcquireBatchAsync(
        int take,
        TimeSpan lease,
        DateTime utcNow,
        string leaseOwner,
        CancellationToken ct);

    Task<ServerTransportApplyJobDto?> GetAsync(Guid jobId, CancellationToken ct);

    Task<int> MarkCompletedAsync(
        IReadOnlyCollection<Guid> jobIds,
        string leaseOwner,
        DateTime utcNow,
        CancellationToken ct);

    Task<int> FailPermanentAsync(
        IReadOnlyCollection<ServerTransportApplyJobFailure> failures,
        string leaseOwner,
        DateTime utcNow,
        CancellationToken ct);

    Task<int> FailOrRescheduleAsync(
        IReadOnlyCollection<ServerTransportApplyJobFailure> failures,
        string leaseOwner,
        DateTime utcNow,
        Func<int, TimeSpan> backoff,
        CancellationToken ct);

    Task<int> MarkObsoleteAsync(
        IReadOnlyCollection<Guid> jobIds,
        string leaseOwner, 
        DateTime utcNow,
        CancellationToken ct);
}
