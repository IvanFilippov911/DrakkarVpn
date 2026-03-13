using DrakkarVpn.Core.Api.Modules.Peers.Application.DTOs;

namespace DrakkarVpn.Core.Api.Modules.Peers.Application.Abstractions;

public interface IPeerConfigsService
{
    Task<PeerConfigDto?> GetByAgentUuidAsync(Guid peerUuid, CancellationToken ct);
}