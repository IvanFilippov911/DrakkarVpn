using DrakkarVpn.Core.Api.Modules.Servers.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Servers.Infrastructure.EF;
using DrakkarVpn.Core.Api.Modules.Servers.Infrastructure.EF.ReadModels;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using NpgsqlTypes;

namespace DrakkarVpn.Core.Api.Modules.Servers.Infrastructure.Repositories;

public sealed class ServerRealtimeStatsRepository : IServerRealtimeStatsRepository
{
    private readonly ServerDbContext _db;

    public ServerRealtimeStatsRepository(ServerDbContext db) => _db = db;

    public async Task UpsertManyAsync(
        IReadOnlyCollection<ServerRealtimeStatsUpsertRow> rows,
        DateTime nowUtc,
        CancellationToken ct)
    {
        if (rows.Count == 0) return;

        var serverIds = rows.Select(r => r.ServerId).ToArray();
        var online    = rows.Select(r => r.OnlinePeers).ToArray();
        var t1h       = rows.Select(r => r.TrafficLast1hBytes).ToArray();
        var t24h      = rows.Select(r => r.TrafficLast24hBytes).ToArray();
        var updated   = rows.Select(_ => nowUtc).ToArray();

        const string sql = """
            INSERT INTO server_realtime_stats(
                server_id,
                online_peers,
                traffic_last_1h_bytes,
                traffic_last_24h_bytes,
                updated_at_utc,
                traffic_calculated_at_utc
            )
            SELECT *
            FROM UNNEST(
                @server_ids::uuid[],
                @online_peers::int[],
                @t1h::bigint[],
                @t24h::bigint[],
                @updated_at::timestamptz[],
                @calculated_at::timestamptz[]
            ) AS t(
                server_id,
                online_peers,
                traffic_last_1h_bytes,
                traffic_last_24h_bytes,
                updated_at_utc,
                traffic_calculated_at_utc
            )
            ON CONFLICT (server_id) DO UPDATE SET
                online_peers = EXCLUDED.online_peers,
                traffic_last_1h_bytes = EXCLUDED.traffic_last_1h_bytes,
                traffic_last_24h_bytes = EXCLUDED.traffic_last_24h_bytes,
                updated_at_utc = EXCLUDED.updated_at_utc,
                traffic_calculated_at_utc = EXCLUDED.traffic_calculated_at_utc;
            """;

        var conn = (NpgsqlConnection)_db.Database.GetDbConnection();
        await using var _ = conn;
        
        if (conn.State != System.Data.ConnectionState.Open)
            await conn.OpenAsync(ct);

        await using var cmd = new NpgsqlCommand(sql, conn);

        cmd.Parameters.Add(new NpgsqlParameter("server_ids", NpgsqlDbType.Array | NpgsqlDbType.Uuid) { Value = serverIds });
        cmd.Parameters.Add(new NpgsqlParameter("online_peers", NpgsqlDbType.Array | NpgsqlDbType.Integer) { Value = online });
        cmd.Parameters.Add(new NpgsqlParameter("t1h", NpgsqlDbType.Array | NpgsqlDbType.Bigint) { Value = t1h });
        cmd.Parameters.Add(new NpgsqlParameter("t24h", NpgsqlDbType.Array | NpgsqlDbType.Bigint) { Value = t24h });
        cmd.Parameters.Add(new NpgsqlParameter("updated_at", NpgsqlDbType.Array | NpgsqlDbType.TimestampTz) { Value = updated });
        cmd.Parameters.Add(new NpgsqlParameter("calculated_at", NpgsqlDbType.Array | NpgsqlDbType.TimestampTz) { Value = updated });

        await cmd.ExecuteNonQueryAsync(ct);
    }
}