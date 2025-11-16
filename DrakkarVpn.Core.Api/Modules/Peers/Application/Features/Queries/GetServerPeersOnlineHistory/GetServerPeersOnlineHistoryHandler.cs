using DrakkarVpn.Core.Api.Modules.Peers.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Servers.Application.Features.Queries.GetServers;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Peers.Application.Features.Queries.GetServerPeersOnlineHistory;

public sealed class GetServerPeersOnlineHistoryHandler
    : IRequestHandler<GetServerPeersOnlineHistoryQuery, IReadOnlyList<ServerOnlinePointDto>>
{
    private readonly IPeerMetricsHistoryRepository _repo;

    public GetServerPeersOnlineHistoryHandler(IPeerMetricsHistoryRepository repo)
        => _repo = repo;

    public Task<IReadOnlyList<ServerOnlinePointDto>> Handle(
        GetServerPeersOnlineHistoryQuery q,
        CancellationToken ct)
    {
        var toUtc   = DateTime.UtcNow;
        var fromUtc = toUtc.AddMinutes(-q.Minutes);

        return _repo.GetServerPeersOnlineTimelineAsync(q.ServerId, fromUtc, toUtc, ct);
    }
}