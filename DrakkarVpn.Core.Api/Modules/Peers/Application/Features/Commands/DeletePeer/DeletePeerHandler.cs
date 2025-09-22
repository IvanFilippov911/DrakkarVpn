using DrakkarVpn.Core.Api.Modules.Peers.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Peers.Domain;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Peers.Application.Features.Commands.DeletePeer;

public sealed class DeletePeerHandler : IRequestHandler<DeletePeerRequest, bool>
{
    private readonly IPeerRepository _peers;

    public DeletePeerHandler(IPeerRepository peers) => _peers = peers;

    public async Task<bool> Handle(DeletePeerRequest req, CancellationToken ct)
    {
        var peer = await _peers.GetByIdAsync(new PeerId(req.PeerId), ct);
        if (peer is null)
            return false;

        _peers.Remove(peer);
        return true;
    }
}