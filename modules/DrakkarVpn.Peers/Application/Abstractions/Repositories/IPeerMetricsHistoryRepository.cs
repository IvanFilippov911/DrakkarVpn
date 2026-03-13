using DrakkarVpn.Core.Api.Modules.Admin.Application.DTOs;
using DrakkarVpn.Core.Api.Modules.Peers.Application.DTOs;
using DrakkarVpn.Core.Api.Modules.Peers.Infrastructure.Entities;
using DrakkarVpn.Core.Api.Modules.Servers.Application.Features.Queries.GetServers;

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
    
    Task<IReadOnlyList<ServerOnlinePointDto>> GetServerPeersOnlineTimelineAsync(
        Guid serverId, DateTime fromUtc, DateTime toUtc, CancellationToken ct);
    
    Task<IReadOnlyList<SuspiciousPeerTrafficDto>> GetRecentTrafficSummaryAsync(
        DateTime fromUtc,
        DateTime toUtc,
        CancellationToken ct);

    Task<Dictionary<Guid, long>> GetPeersTrafficBytesOnServerAsync(
        Guid serverId,
        Guid[] peerIds,
        DateTime fromUtc,
        DateTime toUtc,
        CancellationToken ct);
    
    Task<long> GetPeerTrafficBytesAsync(
        Guid peerId,
        DateTime fromUtc,
        DateTime toUtc,
        CancellationToken ct);
    
    Task<IReadOnlyList<PeerTrafficWindowRow>> GetTrafficAggForWindowAsync(
        DateTime fromUtc,
        DateTime toUtc,
        CancellationToken ct);
}