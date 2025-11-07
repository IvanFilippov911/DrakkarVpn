using DrakkarVpn.Core.Api.Modules.Peers.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Peers.Infrastructure.Entities;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Peers.Application.Features.Commands.UpsertPeerMetricsHistory;

public sealed class UpsertPeerMetricsHistoryHandler
    : IRequestHandler<UpsertPeerMetricsHistoryCommand, Unit>
{
    private readonly IPeerMetricsHistoryRepository _repo;

    public UpsertPeerMetricsHistoryHandler(IPeerMetricsHistoryRepository repo)
    {
        _repo = repo;
    }

    public async Task<Unit> Handle(UpsertPeerMetricsHistoryCommand c, CancellationToken ct)
    {
        var entities = c.Items.Select(i => new PeerMetricsHistory
        {
            PeerId         = i.PeerId,
            ServerId       = c.ServerId,
            PeriodStartUtc = c.PeriodStartUtc,
            TotalRxBytes   = i.TotalRxBytes,
            TotalTxBytes   = i.TotalTxBytes,
            IsOnline       = i.IsOnline,
            VpnLatencyMs   = i.VpnLatencyMs,
        }).ToArray();

        await _repo.UpsertRangeAsync(entities, ct);
        return Unit.Value;
    }
}