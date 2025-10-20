using DrakkarVpn.Core.Api.Modules.Servers.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Servers.Infrastructure.Entities;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Servers.Application.Features.Commands.UpsertServerMetricsHistory;

public sealed class UpsertServerMetricsHistoryHandler
    : IRequestHandler<UpsertServerMetricsHistoryRequest, Unit>
{
    private readonly IServerMetricsHistoryRepository _repo;
    public UpsertServerMetricsHistoryHandler(IServerMetricsHistoryRepository repo) => _repo = repo;

    public async Task<Unit> Handle(UpsertServerMetricsHistoryRequest c, CancellationToken ct)
    {
        await _repo.AddOrUpdateMinuteAsync(new ServerMetricsHistory
        {
            PeriodStartUtc  = c.PeriodStartUtc,
            ServerId        = c.ServerId,
            Reachable       = c.Reachable,
            PeersActive     = c.PeersActive,
            MaxPeers        = c.MaxPeers,
            TrafficRxBytes  = c.TrafficRxBytes,
            TrafficTxBytes  = c.TrafficTxBytes,
            VpnSpeedMbps    = c.VpnSpeedMbps,
            InfraLatencyMs  = c.InfraLatencyMs
        }, ct);

        return Unit.Value;
    }
}