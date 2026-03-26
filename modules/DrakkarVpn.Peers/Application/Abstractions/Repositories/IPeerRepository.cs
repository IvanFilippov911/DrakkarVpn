using System.Linq.Expressions;
using DrakkarVpn.Core.Api.Modules.Peers.Application.DTOs;
using DrakkarVpn.Core.Api.Modules.Peers.Domain;

namespace DrakkarVpn.Core.Api.Modules.Peers.Application.Abstractions;

public interface IPeerRepository
{
    Task<Peer?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<List<Peer>> GetListActiveByServerAsync(Guid serverId, CancellationToken ct);
    Task AddAsync(Peer peer, CancellationToken ct);
    Task<IReadOnlyList<Peer>> GetByServerAsync(Guid serverId, CancellationToken ct);
    
    Task<Dictionary<Guid, int>> GetOnlineCountsByServerAsync(
        Guid[] serverIds,
        CancellationToken ct);

    Task<int> GetActiveCountByServerIdAsync(Guid serverId, CancellationToken ct);
    
    Task<Peer?> GetByAgentUuidAsync(Guid uuid, CancellationToken ct);
    Task<Peer?> GetPeerForConfigByAgentUuidAsync(Guid agentUuid, CancellationToken ct);
    
    Task<Peer?> GetByDeviceIdAsync(string deviceId, CancellationToken ct);

    Task<IReadOnlyList<T>> GetForRevokeByUserAsync<T>(
        Guid userId,
        Expression<Func<Peer, T>> selector,
        CancellationToken ct);
    
    
    Task<int> CountOnServerAsync(Guid serverId, CancellationToken ct);
    
    Task<(IReadOnlyList<PeerRow> Items, int Total)> GetServerPeersAsync(
        Guid serverId,
        int page,
        int pageSize,
        CancellationToken ct);
    
    Task<PeerRow?> GetPeerAsync(
        Guid peerId,
        CancellationToken ct);

    Task<IReadOnlyList<PeerRevokeCandidateRow>> GetActiveForRevokeByUsersAsync(
        IReadOnlyCollection<Guid> userIds,
        CancellationToken ct);

    Task RevokedManyAsync(
        IReadOnlyCollection<Guid> peerIds,
        DateTime markerUtc,
        CancellationToken ct);

    Task<IReadOnlyList<Guid>> GetRevokedPeerIdsByMarkerAsync(
        IReadOnlyCollection<Guid> peerIds,
        DateTime markerUtc,
        CancellationToken ct);
    
    Task<ActivePeerForDeviceDto?> GetActiveForDeviceAsync(string deviceId, CancellationToken ct);

    Task MarkRevokedAsync(Guid peerId, DateTime markerUtc, CancellationToken ct);
    Task<bool> IsRevokedByMarkerAsync(Guid peerId, DateTime markerUtc, CancellationToken ct);
    
    Task<IReadOnlyDictionary<string, Guid>> GetPeerIdsByDeviceIdsAsync(
        IReadOnlyCollection<string> deviceIds,
        CancellationToken ct);

    Task CreateManyIgnoreConflictsAsync(
        IReadOnlyCollection<Peer> peers,
        CancellationToken ct);
    

}