using DrakkarVpn.Core.Api.Modules.Peers.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Servers.Application.Features.Queries.GetServers;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Queries.Servers.GetServerPeersOnlineHistory;

public sealed class GetServerPeersOnlineHistoryHandler
    : IRequestHandler<GetServerPeersOnlineHistoryQuery, IReadOnlyList<ServerOnlinePointDto>>
{
    private readonly IPeersQueryService _peers;

    public GetServerPeersOnlineHistoryHandler(IPeersQueryService peers)
        => _peers = peers;

    public Task<IReadOnlyList<ServerOnlinePointDto>> Handle(
        GetServerPeersOnlineHistoryQuery q,
        CancellationToken ct)
        => _peers.GetServerPeersOnlineHistoryAsync(q.ServerId, q.Minutes, ct);
    
}