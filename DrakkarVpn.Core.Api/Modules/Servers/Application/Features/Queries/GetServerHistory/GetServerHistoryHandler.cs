using DrakkarVpn.Core.Api.Modules.Servers.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Servers.Application.Features.Queries.GetServers;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Servers.Application.Features.Queries.GetServerHistory;

public sealed class GetServerHistoryHandler
    : IRequestHandler<GetServerHistoryRequest, IReadOnlyList<ServerMetricsHistoryDto>>
{
    private readonly IServerMetricsHistoryRepository _repo;
    public GetServerHistoryHandler(IServerMetricsHistoryRepository repo) => _repo = repo;

    public async Task<IReadOnlyList<ServerMetricsHistoryDto>> Handle(GetServerHistoryRequest q, CancellationToken ct)
    {
        var toUtc   = (q.ToUtc   ?? DateTime.UtcNow).ToUniversalTime();
        var fromUtc = (q.FromUtc ?? toUtc.AddHours(-24)).ToUniversalTime();

        var rows = await _repo.GetRangeAsync(q.ServerId, fromUtc, toUtc, ct);
        return rows.Select(x => 
            new ServerMetricsHistoryDto(
            x.PeriodStartUtc, 
            x.ServerId, 
            x.Reachable,
            x.TrafficRxBytes, 
            x.TrafficTxBytes, 
            x.VpnSpeedMbps, 
            x.InfraLatencyMs)).ToList();
    }
}