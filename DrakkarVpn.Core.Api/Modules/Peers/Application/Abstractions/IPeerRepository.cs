using DrakkarVpn.Core.Api.Modules.Peers.Application.DTOs;
using DrakkarVpn.Core.Api.Modules.Peers.Domain;
using DrakkarVpn.Core.Api.Modules.Subscriptions.Domain.ValueObjects;

namespace DrakkarVpn.Core.Api.Modules.Peers.Application.Abstractions;

public interface IPeerRepository
{
    Task<Peer?> GetByIdAsync(PeerId id, CancellationToken ct);
    Task AddAsync(Peer peer, CancellationToken ct);
    Task<IReadOnlyList<Peer>> GetByUserAsync(Guid userId, CancellationToken ct);
    Task<IReadOnlyList<Peer>> GetByServerAsync(Guid serverId, CancellationToken ct);

    Task<IReadOnlyList<Peer>> GetAllAsync(int limit, int offset, CancellationToken ct);

    IQueryable<Peer> Query();

    Task<int> GetActiveCountByServerIdAsync(Guid serverId, CancellationToken ct);

    void Remove(Peer peer);
    Task<IReadOnlyList<Peer>> GetBySubscriptionAsync(SubscriptionId subscriptionId, CancellationToken ct);
    Task<Peer?> GetByAgentUuidAsync(AgentPeerUuid uuid, CancellationToken ct);
    
    Task<int> SaveChangesAsync(CancellationToken ct);
    Task<Peer?> GetActiveBySubscriptionAndDeviceAsync(SubscriptionId subId, string deviceId, CancellationToken ct);
}