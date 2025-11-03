using System.Linq.Expressions;
using DrakkarVpn.Core.Api.Modules.Peers.Domain;
using DrakkarVpn.Core.Api.Modules.Subscriptions.Domain.ValueObjects;
using DrakkarVpn.Shared.Peers;

namespace DrakkarVpn.Core.Api.Modules.Peers.Application.Abstractions;

public interface IPeerRepository
{
    Task<Peer?> GetByIdAsync(Guid id, CancellationToken ct);
    Task AddAsync(Peer peer, CancellationToken ct);
    Task<IReadOnlyList<Peer>> GetByServerAsync(Guid serverId, CancellationToken ct);

    Task<IReadOnlyList<Peer>> GetAllAsync(int limit, int offset, CancellationToken ct);

    IQueryable<Peer> Query();

    Task<int> GetActiveCountByServerIdAsync(Guid serverId, CancellationToken ct);

    void Remove(Peer peer);
    Task<Peer?> GetByAgentUuidAsync(Guid uuid, CancellationToken ct);
    
    Task<int> SaveChangesAsync(CancellationToken ct);

    Task<Peer?> GetByDeviceIdAsync(string deviceId, CancellationToken ct);
    
    Task<IReadOnlyList<T>> GetForRevokeAsync<T>(
        Guid subscriptionId,
        Expression<Func<Peer, T>> selector,
        CancellationToken ct);
    
    Task<Dictionary<string, PeerBriefDto>> GetMapByDeviceIdsOnServerAsync(
        Guid serverId,
        IReadOnlyCollection<string> deviceIds,
        CancellationToken ct);
    

}