using DrakkarVpn.Core.Api.Modules.Peers.Infrastructure.Entities;

namespace DrakkarVpn.Core.Api.Modules.Peers.Application.Abstractions;

public interface IPeerMetricsHistoryRepository
{
    Task UpsertRangeAsync(
        IReadOnlyCollection<PeerMetricsHistory> items,
        CancellationToken ct);

    Task DeleteOlderThanAsync(TimeSpan ttl, CancellationToken ct);
    
    Task<IReadOnlyList<PeerMetricsHistory>> GetRangeAsync(
        Guid peerId,
        DateTime fromUtc,
        DateTime toUtc,
        CancellationToken ct);
}