using DrakkarVpn.Core.Api.Modules.Peers.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Peers.Application.DTOs;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Peers.Application.Features.Queries.GetPeersForRevoke;

public sealed class GetPeersForRevokeHandler
    : IRequestHandler<GetPeersForRevokeQuery, IReadOnlyList<PeerForRevokeDto>>
{
    private readonly IPeerRepository _peers;

    public GetPeersForRevokeHandler(IPeerRepository peers) => _peers = peers;

    public Task<IReadOnlyList<PeerForRevokeDto>> Handle(GetPeersForRevokeQuery req, CancellationToken ct)
        => _peers.GetForRevokeAsync(
            req.SubscriptionId,
            p => new PeerForRevokeDto(
                p.Id,
                p.ServerId,
                (int)p.Status
            ),
            ct);
}