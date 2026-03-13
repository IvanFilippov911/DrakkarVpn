using DrakkarVpn.Core.Api.Modules.Peers.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Peers.Application.DTOs;
using DrakkarVpn.Shared;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Queries.Peers.GetServerPeers;

public sealed class GetServerPeersHandler
    : IRequestHandler<GetServerPeersQuery, PagedResponseDto<ServerPeerDto>>
{
    private readonly IPeersQueryService _peers;

    public GetServerPeersHandler(IPeersQueryService peers)
        => _peers = peers;

    public Task<PagedResponseDto<ServerPeerDto>> Handle(
        GetServerPeersQuery q,
        CancellationToken ct)
        => _peers.GetServerPeersAsync(
            q.ServerId,
            q.Page,
            q.PageSize,
            q.OnlyOnline ?? false,
            q.PeerId,
            q.SortBy,
            q.PeerSortDirection,
            ct);
}