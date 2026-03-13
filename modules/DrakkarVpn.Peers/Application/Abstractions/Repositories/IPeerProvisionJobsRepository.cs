using DrakkarVpn.Core.Api.Modules.Peers.Application.DTOs;
using DrakkarVpn.Core.Api.Modules.Peers.Infrastructure.Entities;

namespace DrakkarVpn.Core.Api.Modules.Peers.Application.Abstractions;

public interface IPeerProvisionJobsRepository
{
    Task<Guid> CreateOrGetAsync(PeerProvisionJobCreateDto dto, DateTime nowUtc, CancellationToken ct);
    Task<PeerProvisionJob?> GetByIdAsync(Guid jobId, CancellationToken ct);

    Task<IReadOnlyList<PeerProvisionJob>> AcquireBatchAsync(
        int take,
        TimeSpan lease,
        DateTime nowUtc,
        string instanceId,
        CancellationToken ct);

    Task MarkAgentAppliedAsync(Guid jobId, DateTime nowUtc, CancellationToken ct);

    Task MarkReadyAsync(
        Guid jobId,
        Guid peerId,
        DateTime nowUtc,
        CancellationToken ct);

    Task RescheduleAsync(
        Guid jobId,
        int newAttempt,
        DateTime nextAttemptAtUtc,
        string code,
        string? message,
        DateTime nowUtc,
        CancellationToken ct);

    Task MarkFailedAsync(
        Guid jobId,
        string code,
        string? message,
        DateTime nowUtc,
        CancellationToken ct);
}