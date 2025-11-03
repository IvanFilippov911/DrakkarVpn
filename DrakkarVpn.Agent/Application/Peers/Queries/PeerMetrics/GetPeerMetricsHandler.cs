using DrakkarVpn.Agent.Application.Abstractions;
using DrakkarVpn.Agent.Application.Peers.Services;
using DrakkarVpn.Shared.Peers;
using MediatR;

namespace DrakkarVpn.Agent.Application.Peers.Queries.PeerMetrics;

public sealed class GetPeerMetricsHandler
    : IRequestHandler<GetPeerMetricsQuery, IReadOnlyList<PeerMetricsDto>>
{
    private readonly IXrayPeerClient _peers;
    private readonly IXrayStatsClient _stats;

    public GetPeerMetricsHandler(
        IXrayPeerClient peers,
        IXrayStatsClient stats)
    {
        _peers = peers;
        _stats = stats;
    }

    public async Task<IReadOnlyList<PeerMetricsDto>> Handle(GetPeerMetricsQuery request, CancellationToken ct)
    {
        var users = await _peers.GetListPeersAsync(ct);
        if (users.Count == 0)
            return Array.Empty<PeerMetricsDto>();

        var rawStats = await _stats.GetUserStatsAsync(ct);
        var byEmail  = XrayUserStatsParser.BuildTrafficByEmail(rawStats);
        
        var result = new List<PeerMetricsDto>(users.Count);

        foreach (var u in users)
        {
            if (u.Uuid == Guid.Empty)
                continue;

            byEmail.TryGetValue(u.Email, out var x);

            result.Add(new PeerMetricsDto(
                AgentPeerId:    u.Uuid,
                RxBytesTotal:   x.up,
                TxBytesTotal:   x.down,
                VpnLatencyMs:   null
            ));
        }

        return result;
    }
}