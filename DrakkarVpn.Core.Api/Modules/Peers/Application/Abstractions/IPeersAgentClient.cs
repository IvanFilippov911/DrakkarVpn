using DrakkarVpn.Core.Api.Modules.Peers.Application.DTOs;
using DrakkarVpn.Core.Api.Modules.Peers.Domain;
using DrakkarVpn.Core.Api.Modules.Servers.Domain;

namespace DrakkarVpn.Core.Api.Modules.Peers.Application.Abstractions;

public interface IPeersAgentClient
{
    Task<PeerAgentRegisterResponseDto> RegisterPeerAsync(Server server, CancellationToken ct);
    Task<bool> RevokePeerAsync(Server server, Guid? peerUuid, CancellationToken ct);
}