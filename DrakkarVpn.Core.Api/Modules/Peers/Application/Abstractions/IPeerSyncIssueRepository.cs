using DrakkarVpn.Core.Api.Modules.Peers.Application.DTOs;

namespace DrakkarVpn.Core.Api.Modules.Peers.Application.Abstractions;

public interface IPeerSyncIssueRepository
{
    Task InsertManyAsync(Guid serverId, IReadOnlyCollection<PeerSyncIssueItem> items, CancellationToken ct);
}