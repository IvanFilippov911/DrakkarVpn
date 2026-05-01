using DrakkarVpn.Agent.Application.Abstractions;
using MediatR;

namespace DrakkarVpn.Agent.Application.Peers.Commads.RevokePeer;

public sealed class RevokePeerHandler 
    : IRequestHandler<RevokePeerCommand, bool>
{
    private readonly IXrayPeerClient _v2ray;

    public RevokePeerHandler(IXrayPeerClient v2ray)
    {
        _v2ray = v2ray;
    }

    public Task<bool> Handle(RevokePeerCommand request, CancellationToken ct)
        => _v2ray.RevokePeerAsync(request.PeerUuid, ct);
}