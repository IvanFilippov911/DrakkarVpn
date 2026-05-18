using DrakkarVpn.Admin.Api.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Admin.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Peers.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Servers.Application.Abstractions;
using DrakkarVpn.Servers.Application.Abstractions.Services;
using DrakkarVpn.Core.Api.Modules.Servers.Infrastructure.EF.ReadModels;
using DrakkarVpn.Servers.Application.Abstractions.Services.Queries;

namespace DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Services;

public sealed class ServerRealtimeStatsUpdater : IServerRealtimeStatsUpdater
{
    private readonly IServerConfigQueryService _serverConfig;
    private readonly IServerMetricsQueryService _serverMetrics;
    private readonly IPeersQueryService _peersQuery;
    private readonly IServerRealtimeStatsUpsertService _upsert;

    public ServerRealtimeStatsUpdater(
        IServerConfigQueryService serverConfig,
        IServerMetricsQueryService serverMetrics,
        IPeersQueryService peersQuery,
        IServerRealtimeStatsUpsertService upsert)
    {
        _serverConfig = serverConfig;
        _serverMetrics = serverMetrics;
        _peersQuery = peersQuery;
        _upsert = upsert;
    }

    public async Task<int> UpdateNowAsync(DateTime nowUtc, CancellationToken ct)
    {
        var from1h  = nowUtc.AddHours(-1);
        var from24h = nowUtc.AddHours(-24);

        var serverIds = await _serverConfig.GetEnabledServerIdsAsync(ct);
        if (serverIds.Length == 0)
            return 0;

        var onlineDict     = await _peersQuery.GetServersOnlinePeersSummaryAsync(serverIds, ct);
        var traffic1hDict  = await _serverMetrics.GetTrafficSummaryAsync(serverIds, from1h, ct);
        var traffic24hDict = await _serverMetrics.GetTrafficSummaryAsync(serverIds, from24h, ct);

        var rows = new List<ServerRealtimeStatsUpsertRow>(serverIds.Length);

        foreach (var id in serverIds)
        {
            onlineDict.TryGetValue(id, out var online);
            traffic1hDict.TryGetValue(id, out var t1);
            traffic24hDict.TryGetValue(id, out var t24);

            rows.Add(new ServerRealtimeStatsUpsertRow(
                ServerId: id,
                OnlinePeers: online,
                TrafficLast1hBytes: t1,
                TrafficLast24hBytes: t24));
        }

        await _upsert.UpsertManyAsync(rows, nowUtc, ct);
        return rows.Count;
    }
}