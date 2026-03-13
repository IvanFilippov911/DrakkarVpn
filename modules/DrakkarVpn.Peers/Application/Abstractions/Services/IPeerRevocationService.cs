using DrakkarVpn.Core.Api.Modules.Peers.Application.DTOs;

namespace DrakkarVpn.Core.Api.Modules.Peers.Application.Abstractions;

public interface IPeerRevocationService
{
    Task<bool> RevokePeerAsync(Guid serverId, Guid peerId, CancellationToken ct);
    Task<BulkPeersRevokeResultDto> RevokeUsersPeersAsync(
        IReadOnlyCollection<Guid> userIds,
        DateTime markerUtc,
        CancellationToken ct);
    Task<int> RevokeAllServerPeersAsync(Guid serverId, CancellationToken ct);
}