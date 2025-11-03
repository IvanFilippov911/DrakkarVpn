using DrakkarVpn.Core.Api.Modules.Peers.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Peers.Application.DTOs;
using DrakkarVpn.Core.Api.Modules.Peers.Domain;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Peers.Application.Features.Queries.GetPeerByUuid;

public sealed class GetPeerByUuidHandler 
    : IRequestHandler<GetPeerByUuidRequest, GetPeerByUuidResponse?>
{
    private readonly IPeerRepository _peers;

    public GetPeerByUuidHandler(IPeerRepository peers) => _peers = peers;

    public async Task<GetPeerByUuidResponse?> Handle(GetPeerByUuidRequest req, CancellationToken ct)
    {
        var peer = await _peers.GetByAgentUuidAsync(req.PeerUuid, ct);
        if (peer is null) return null;

        return new GetPeerByUuidResponse(peer.ConfigRaw);
    }
}