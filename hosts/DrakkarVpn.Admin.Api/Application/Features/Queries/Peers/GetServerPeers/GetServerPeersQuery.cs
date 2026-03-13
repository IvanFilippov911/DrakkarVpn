using DrakkarVpn.Core.Api.Modules.Peers.Application.DTOs;
using DrakkarVpn.Core.Api.Modules.Peers.Domain.enums;
using DrakkarVpn.Shared;
using DrakkarVpn.Shared.Servers;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Queries.Peers.GetServerPeers;

public sealed record GetServerPeersQuery(
    Guid ServerId,
    int Page,
    int PageSize,
    bool? OnlyOnline,
    Guid? PeerId,
    ServerPeerSortBy SortBy,
    SortDirection PeerSortDirection
) : IRequest<PagedResponseDto<ServerPeerDto>>;