using DrakkarVpn.Core.Api.Modules.Peers.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Peers.Domain;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Peers.Application.Features.Commands.RenewPeer;

public sealed class RenewPeerHandler : IRequestHandler<RenewPeerRequest, bool>
{
    private readonly IPeerRepository _peers;

    public RenewPeerHandler(IPeerRepository peers) => _peers = peers;

    public async Task<bool> Handle(RenewPeerRequest req, CancellationToken ct)
    {
        var peer = await _peers.GetByIdAsync(new PeerId(req.PeerId), ct);
        if (peer is null)
            throw new InvalidOperationException($"Peer {req.PeerId} not found");

        if (!peer.IsActive())
            throw new InvalidOperationException($"Peer {req.PeerId} is not active and cannot be renewed");

        peer.Renew(req.NewExpiresAt);
        
        return true;
    }
}