using DrakkarVpn.Core.Api.Modules.Peers.Application.DTOs;
using DrakkarVpn.Core.Api.Modules.Peers.Domain;
using DrakkarVpn.Core.Api.Modules.Servers.Domain;

namespace DrakkarVpn.Core.Api.Modules.Peers.Application.Abstractions;

public interface IAgentClient
{
    Task<PeerAgentRegisterResponseDto> RegisterPeerAsync(Server server, Guid userId, CancellationToken ct);
    Task<bool> RevokePeerAsync(Server server, AgentPeerUuid peerUuid, CancellationToken ct);
}