using DrakkarVpn.Core.Api.Modules.Peers.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Peers.Domain;
using DrakkarVpn.Core.Api.Modules.Servers.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Servers.Domain;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Peers.Application.Features.Commands.RevokePeer;

public sealed class RevokePeerHandler : IRequestHandler<RevokePeerRequest, bool>
{
    private readonly IPeerRepository _peers;
    private readonly IServerRepository _servers;
    private readonly IAgentClient _agentClient;

    public RevokePeerHandler(
        IPeerRepository peers,
        IServerRepository servers,
        IAgentClient agentClient)
    {
        _peers = peers;
        _servers = servers;
        _agentClient = agentClient;
    }

    public async Task<bool> Handle(RevokePeerRequest req, CancellationToken ct)
    {
        var peer = await _peers.GetByIdAsync(new PeerId(req.PeerId), ct);
        if (peer is null || !peer.IsActive())
            return false;
        
        var server = await _servers.GetAsync(new ServerId(req.ServerId), ct);
        if (server is null)
            throw new InvalidOperationException($"Server {req.ServerId} not found");
        
        var agentOk = await _agentClient.RevokePeerAsync(server, peer.AgentPeerUuid, ct);
        if (!agentOk)
            throw new InvalidOperationException($"Agent {server.Id} refused to revoke peer {peer.Id}");
        
        peer.Revoke();

        return true;
    }
}