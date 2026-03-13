using DrakkarVpn.Core.Api.Modules.Peers.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Peers.Application.DTOs;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Queries.Peers.GetPeerHistory;

public sealed class GetPeerHistoryHandler
    : IRequestHandler<GetPeerHistoryRequest, IReadOnlyList<PeerMetricsHistoryDto>>
{
    private readonly IPeersQueryService _peers;

    public GetPeerHistoryHandler(IPeersQueryService peers) => _peers = peers;

    public Task<IReadOnlyList<PeerMetricsHistoryDto>> Handle(GetPeerHistoryRequest q, CancellationToken ct)
        => _peers.GetPeerHistoryAsync(q.PeerId, q.FromUtc, q.ToUtc, ct);
}