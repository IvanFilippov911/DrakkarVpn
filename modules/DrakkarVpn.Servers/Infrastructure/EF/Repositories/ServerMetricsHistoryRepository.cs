using System.Data;
using DrakkarVpn.Core.Api.Modules.Servers.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Servers.Infrastructure.EF;
using DrakkarVpn.Core.Api.Modules.Servers.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace DrakkarVpn.Core.Api.Modules.Servers.Infrastructure.Repositories;

public sealed class ServerMetricsHistoryRepository : IServerMetricsHistoryRepository
{
    private readonly ServerDbContext _db;
    public ServerMetricsHistoryRepository(ServerDbContext db) => _db = db;

    public async Task AppendFromStateAsync(
    IReadOnlyCollection<Guid> appliedServerIds,
    DateTime periodStartUtc,
    DateTime nowUtc,
    CancellationToken ct)
    {
        if (appliedServerIds is null) throw new ArgumentNullException(nameof(appliedServerIds));
        if (appliedServerIds.Count == 0) return;

        periodStartUtc = DateTime.SpecifyKind(periodStartUtc, DateTimeKind.Utc);
        nowUtc         = DateTime.SpecifyKind(nowUtc, DateTimeKind.Utc);

        const string sql = """
            INSERT INTO servers.server_metrics_history (
                period_start,
                server_id,
                reachable,
                traffic_rx_delta_bytes,
                traffic_tx_delta_bytes,
                vpn_speed_mbps,
                infra_latency_ms
            )
            SELECT
                @period_start_utc,
                sps."ServerId",
                COALESCE(sps."LastPollSuccess", false)
                    AND COALESCE(sr.health_reachable, false) AS reachable,
                sps."LastRxDelta",
                sps."LastTxDelta",
                sr.metrics_vpn_speed_mbps,
                sr.metrics_infra_latency_ms
            FROM servers.server_poll_states sps
            JOIN servers.servers sr ON sr.id = sps."ServerId"
            WHERE sps."ServerId" = ANY(@server_ids::uuid[])
            ON CONFLICT (server_id, period_start)
            DO UPDATE SET
                reachable              = EXCLUDED.reachable,
                traffic_rx_delta_bytes = EXCLUDED.traffic_rx_delta_bytes,
                traffic_tx_delta_bytes = EXCLUDED.traffic_tx_delta_bytes,
                vpn_speed_mbps         = EXCLUDED.vpn_speed_mbps,
                infra_latency_ms       = EXCLUDED.infra_latency_ms;
            """;

        // IMPORTANT: do not dispose the connection that belongs to the EF Core DbContext.
        // Disposing it may break subsequent repository calls within the same DI scope.
        var conn = (NpgsqlConnection)_db.Database.GetDbConnection();
        if (conn.State != ConnectionState.Open)
            await conn.OpenAsync(ct);

        await using var cmd = conn.CreateCommand();
        cmd.CommandText = sql;

        cmd.Parameters.Add(new NpgsqlParameter("period_start_utc", periodStartUtc));
        cmd.Parameters.Add(new NpgsqlParameter("server_ids", appliedServerIds.Distinct().ToArray()));

        await cmd.ExecuteNonQueryAsync(ct);
    }

    public Task DeleteOlderThanAsync(TimeSpan ttl, CancellationToken ct)
    {
        var border = DateTime.UtcNow - ttl;
        return _db.ServersMetricsHistories
            .Where(x => x.PeriodStartUtc < border)
            .ExecuteDeleteAsync(ct); 
    }
    
    public async Task<Dictionary<Guid, long>> GetTrafficSumAsync(
        Guid[] serverIds,
        DateTime fromUtc,
        CancellationToken ct)
    {
        if (serverIds is null || serverIds.Length == 0)
            return new();

        var rows = await _db.ServersMetricsHistories
            .AsNoTracking()
            .Where(x => x.PeriodStartUtc >= fromUtc &&
                        serverIds.Contains(x.ServerId))
            .GroupBy(x => x.ServerId)
            .Select(g => new
            {
                ServerId = g.Key,
                Traffic = g.Sum(r => r.TrafficRxDeltaBytes + r.TrafficTxDeltaBytes)
            })
            .ToListAsync(ct);

        return rows.ToDictionary(x => x.ServerId, x => x.Traffic);
    }
    
    public Task<List<ServerMetricsHistory>> GetRangeAsync(
        Guid serverId,
        DateTime fromUtc,
        DateTime toUtc,
        CancellationToken ct)
    {
        return _db.ServersMetricsHistories
            .AsNoTracking()
            .Where(x => x.ServerId == serverId
                        && x.PeriodStartUtc >= fromUtc
                        && x.PeriodStartUtc <= toUtc)
            .OrderBy(x => x.PeriodStartUtc)
            .ToListAsync(ct);
    }
    
    
    
}
