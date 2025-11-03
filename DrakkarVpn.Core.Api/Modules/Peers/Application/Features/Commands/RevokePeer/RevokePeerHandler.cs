using DrakkarVpn.Core.Api.Modules.Peers.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Peers.Domain;
using DrakkarVpn.Core.Api.Modules.Servers.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Servers.Domain;
using DrakkarVpn.Core.Api.Modules.Servers.Domain.VO;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DrakkarVpn.Core.Api.Modules.Peers.Application.Features.Commands.RevokePeer;

public sealed class RevokePeerHandler : IRequestHandler<RevokePeerRequest, bool>
{
    private readonly IPeerRepository _peers;
    private readonly IServerRepository _servers;
    private readonly IPeersAgentClient _agent;

    public RevokePeerHandler(
        IPeerRepository peers,
        IServerRepository servers,
        IPeersAgentClient agent)
    {
        _peers = peers;
        _servers = servers;
        _agent = agent;
    }

    public async Task<bool> Handle(RevokePeerRequest req, CancellationToken ct)
    {
        var peer = await _peers.GetByIdAsync(req.PeerId, ct);
        if (peer is null) return false;
        
        if (!peer.IsActive()) return true;
        
        if (peer.ServerId != req.ServerId)
            throw new InvalidOperationException(
                $"Peer {peer.Id} belongs to server {peer.ServerId}, not {req.ServerId}");

        var server = await _servers.GetAsync(peer.ServerId, ct)
                    ?? throw new InvalidOperationException($"Server {peer.ServerId} not found");
        
        var agentOk = await _agent.RevokePeerAsync(server, peer.AgentPeerUuid, ct);
        if (!agentOk)
            throw new InvalidOperationException(
                $"Agent {server.Id} refused to revoke peer {peer.Id}");
        
        peer.Revoke();
        await _peers.SaveChangesAsync(ct);

        return true;
    }
}
