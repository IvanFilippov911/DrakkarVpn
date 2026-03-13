using DrakkarVpn.Core.Api.Modules.Peers.Application.DTOs;
using DrakkarVpn.Core.Api.Modules.Peers.Infrastructure.Entities;

namespace DrakkarVpn.Core.Api.Modules.Peers.Application.Abstractions;

public interface IPeerProvisionJobsService
{
    Task<Guid> EnqueueAsync(
        PeerProvisionJobCreateDto dto,
        DateTime nowUtc,
        CancellationToken ct);

    Task<PeerProvisionJobDto?> GetAsync(Guid jobId, CancellationToken ct);

    Task<IReadOnlyList<PeerProvisionJob>> AcquireBatchAsync(
        int take,
        TimeSpan lease,
        DateTime nowUtc,
        string instanceId,
        CancellationToken ct);

    Task MarkAgentAppliedAsync(
        Guid jobId,
        DateTime nowUtc,
        CancellationToken ct);

    Task MarkReadyAsync(
        Guid jobId,
        Guid peerId,
        DateTime nowUtc,
        CancellationToken ct);

    /// <summary>
    /// Удобный метод: сам решает Reschedule или Failed в зависимости от maxAttempts.
    /// </summary>
    Task FailOrRescheduleAsync(
        PeerProvisionJob jobSnapshot,
        string errorCode,
        string? errorMessage,
        DateTime nowUtc,
        Func<int, TimeSpan> backoff,
        CancellationToken ct);
    
    Task FailPermanentAsync(
        PeerProvisionJob jobSnapshot,
        string errorCode,
        string? errorMessage,
        DateTime nowUtc,
        CancellationToken ct);
}