using DrakkarVpn.Core.Api.Modules.Peers.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Peers.Domain;
using DrakkarVpn.Core.Api.Modules.Servers.Domain;

namespace DrakkarVpn.Core.Api.Modules.Peers.Application.Services;

public sealed class PeerRevoker : IPeerRevoker
{
    private readonly IPeersAgentClient _agent;
    private readonly ILogger<PeerRevoker> _logger;

    public PeerRevoker(IPeersAgentClient agent, ILogger<PeerRevoker> logger)
    {
        _agent = agent;
        _logger = logger;
    }

    public async Task<bool> RevokeAsync(Peer peer, Server server, CancellationToken ct)
    {
        if (!peer.IsActive())
            return true;
        
        try
        {
            await _agent.RevokePeerAsync(server, peer.AgentPeerUuid, ct);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogWarning(ex,
                "Failed to revoke peer {PeerId} on agent {ServerId}",
                peer.Id, server.Id);
            return false;
        }
        
        peer.Revoke();
        return true;
    }
}