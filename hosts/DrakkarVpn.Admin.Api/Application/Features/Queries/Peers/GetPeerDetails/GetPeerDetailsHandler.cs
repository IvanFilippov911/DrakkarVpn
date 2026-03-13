using DrakkarVpn.Core.Api.Modules.Peers.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Peers.Application.DTOs;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Queries.Peers.GetPeerDetails;

public sealed class GetPeerDetailsHandler
    : IRequestHandler<GetPeerDetailsQuery, PeerDetailsDto?>
{
    private readonly IPeersQueryService _peers;

    public GetPeerDetailsHandler(IPeersQueryService peers) => _peers = peers;

    public Task<PeerDetailsDto?> Handle(GetPeerDetailsQuery q, CancellationToken ct)
        => _peers.GetPeerDetailsAsync(q.PeerId, ct);
}