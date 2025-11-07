using DrakkarVpn.Core.Api.Modules.Peers.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Peers.Application.DTOs;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Peers.Application.Features.Queries.GetPeerHistory;

public sealed class GetPeerHistoryHandler
    : IRequestHandler<GetPeerHistoryRequest, IReadOnlyList<PeerMetricsHistoryDto>>
{
    private readonly IPeerMetricsHistoryRepository _repo;

    public GetPeerHistoryHandler(IPeerMetricsHistoryRepository repo)
    {
        _repo = repo;
    }

    public async Task<IReadOnlyList<PeerMetricsHistoryDto>> Handle(
        GetPeerHistoryRequest q,
        CancellationToken ct)
    {
        var toUtc   = (q.ToUtc   ?? DateTime.UtcNow).ToUniversalTime();
        var fromUtc = (q.FromUtc ?? toUtc.AddHours(-24)).ToUniversalTime();

        var rows = await _repo.GetRangeAsync(q.PeerId, fromUtc, toUtc, ct);

        return rows
            .Select(x => new PeerMetricsHistoryDto(
                PeriodStartUtc: x.PeriodStartUtc,
                TotalRxBytes:   x.TotalRxBytes,
                TotalTxBytes:   x.TotalTxBytes,
                IsOnline:       x.IsOnline,
                SpeedMbps:      x.SpeedMbps,
                VpnLatencyMs:   x.VpnLatencyMs
            ))
            .ToList();
    }
}