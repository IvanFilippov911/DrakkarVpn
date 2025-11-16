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
    private readonly IPeerRevoker _revoker;

    public RevokePeerHandler(
        IPeerRepository peers,
        IServerRepository servers,
        IPeerRevoker revoker)
    {
        _peers = peers;
        _servers = servers;
        _revoker = revoker;
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
        
        var ok = await _revoker.RevokeAsync(peer, server, ct);
        if (!ok)
            throw new InvalidOperationException(
                $"Agent {server.Id} refused to revoke peer {peer.Id}");
        
        return true;
    }
}
