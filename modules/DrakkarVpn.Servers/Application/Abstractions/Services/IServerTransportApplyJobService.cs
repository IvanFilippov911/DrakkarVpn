using DrakkarVpn.Servers.Application.DTOs.ServerTransportApplyJobs;

namespace DrakkarVpn.Servers.Application.Abstractions.Services;

public interface IServerTransportApplyJobService
{
    Task<Guid> EnqueueAsync(Guid serverId, Guid activationId, CancellationToken ct);

    Task<Guid?> GetActiveJobIdByServerIdAsync(Guid serverId, CancellationToken ct);

    Task<IReadOnlyList<ServerTransportApplyJobDto>> AcquireBatchAsync(
        int take,
        TimeSpan lease,
        DateTime utcNow,
        string leaseOwner,
        CancellationToken ct);

    Task<ServerTransportApplyJobDto?> GetAsync(Guid jobId, CancellationToken ct);

    Task<int> MarkCompletedAsync(
        ServerTransportApplyJobDto jobSnapshot,
        DateTime utcNow,
        CancellationToken ct);

    Task<int> FailPermanentAsync(
        ServerTransportApplyJobDto jobSnapshot,
        string errorCode,
        string? errorMessage,
        DateTime utcNow,
        CancellationToken ct);

    Task<int> FailOrRescheduleAsync(
        ServerTransportApplyJobDto jobSnapshot,
        string errorCode,
        string? errorMessage,
        DateTime utcNow,
        Func<int, TimeSpan> backoff,
        CancellationToken ct);
}
