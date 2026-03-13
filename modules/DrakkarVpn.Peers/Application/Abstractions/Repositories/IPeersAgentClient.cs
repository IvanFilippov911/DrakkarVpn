using DrakkarVpn.Core.Api.Modules.Peers.Application.DTOs;
using DrakkarVpn.Core.Api.Modules.Peers.Domain;
using DrakkarVpn.Core.Api.Modules.Servers.Domain;

namespace DrakkarVpn.Core.Api.Modules.Peers.Application.Abstractions;

public interface IPeersAgentClient
{
    Task RegisterPeerAsync(string agentBaseUrl, Guid peerUuid, string configRaw, CancellationToken ct);
    Task<bool> RevokePeerAsync(string agentBaseUrl, Guid peerUuid, CancellationToken ct);
}