using System.Data;
using DrakkarVpn.Core.Api.Modules.Servers.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Servers.Application.Features.Queries.GetServers;
using DrakkarVpn.Core.Api.Modules.Servers.Application.Features.Queries.GetServers.ServerState;
using DrakkarVpn.Core.Api.Modules.Servers.Domain;
using DrakkarVpn.Core.Api.Modules.Servers.Infrastructure.EF;
using DrakkarVpn.Observability.Application.DTOs;
using DrakkarVpn.Servers.Application.DTOs.ServerState;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using NpgsqlTypes;

namespace DrakkarVpn.Core.Api.Modules.Servers.Infrastructure.Repositories;

public sealed class ServerPollResultApplyRepository : IServerPollResultApplyRepository
{
    private readonly ServerDbContext _db;
    public ServerPollResultApplyRepository(ServerDbContext db) => _db = db;

    
    // Infrastructure/Repositories/ServerPollResultApplyRepository.cs
    public async Task<IReadOnlyDictionary<Guid, ServerState>> GetStateAsync(
        IReadOnlyCollection<Guid> serverIds,
        CancellationToken ct)
    {
        if (serverIds is null) throw new ArgumentNullException(nameof(serverIds));
        if (serverIds.Count == 0) return new Dictionary<Guid, ServerState>();

        const string sql = """
                           SELECT
                               s.id,
                               s.status,
                               ps."ConsecutiveFailures",
                               s.max_peers
                           FROM servers.servers s
                           JOIN servers.server_poll_states ps ON ps."ServerId" = s.id
                           WHERE s.id = ANY(@server_ids);
                           """;

        var conn = (NpgsqlConnection)_db.Database.GetDbConnection();
        if (conn.State != ConnectionState.Open)
            await conn.OpenAsync(ct);

        await using var cmd = conn.CreateCommand();
        cmd.CommandText = sql;
        cmd.Parameters.Add(new NpgsqlParameter("server_ids", serverIds.ToArray()));

        var result = new Dictionary<Guid, ServerState>(serverIds.Count);

        await using var reader = await cmd.ExecuteReaderAsync(ct);
        while (await reader.ReadAsync(ct))
        {
            var serverId = reader.GetGuid(0);
            var status = (ServerStatus)reader.GetInt32(1);
            var consecutiveFailures = reader.GetInt32(2);
            int? maxPeers = reader.IsDBNull(3) ? null : reader.GetInt32(3);

            result[serverId] = new ServerState(
                ServerId: serverId,
                Status: status,
                ConsecutiveFailures: consecutiveFailures,
                MaxPeers: maxPeers
            );
        }

        return result;
    }
    
    
    public async Task<IReadOnlyList<ServerInfoUpdateDto>> ApplyUpdatesAsync(
    IReadOnlyCollection<ServerUpdate> updates,
    CancellationToken ct)
    {
        if (updates is null) throw new ArgumentNullException(nameof(updates));
        if (updates.Count == 0) return Array.Empty<ServerInfoUpdateDto>();

        const string sql = """
            WITH input AS (
                SELECT *
                FROM UNNEST(
                    @server_ids::uuid[],
                    @statuses::int[],
                    @reachable::bool[],
                    @peers_active::int[],
                    @rx_total::bigint[],
                    @tx_total::bigint[],
                    @infra_latency::double precision[],
                    @vpn_speed::double precision[],
                    @updated_at::timestamptz[]
                ) AS r(
                    server_id,
                    status,
                    reachable,
                    peers_active,
                    rx_total,
                    tx_total,
                    infra_latency,
                    vpn_speed,
                    updated_at
                )
            ),
            prev AS (
                SELECT
                    s.id AS server_id,
                    s.status AS old_status,
                    s.max_peers,
                    ps."ConsecutiveFailures" AS consecutive_failures
                FROM servers.servers s
                JOIN servers.server_poll_states ps ON ps."ServerId" = s.id
                JOIN input i ON i.server_id = s.id
            ),
            upd AS (
                UPDATE servers.servers s
                SET
                    status = i.status,
                    health_reachable = i.reachable,
                    health_peers_active = i.peers_active,
                    health_updated_at = i.updated_at,
                    metrics_rx_bytes = i.rx_total,
                    metrics_tx_bytes = i.tx_total,
                    metrics_infra_latency_ms = i.infra_latency,
                    metrics_vpn_speed_mbps = i.vpn_speed,
                    metrics_updated_at = i.updated_at
                FROM input i
                WHERE s.id = i.server_id
                RETURNING s.id
            )
            SELECT
                p.server_id,
                p.old_status,
                i.status AS new_status,
                (i.status = @status_disabled AND p.old_status <> @status_disabled) AS disabled_by_fail,
                i.reachable,
                i.peers_active,
                p.max_peers,
                p.consecutive_failures,
                i.infra_latency,
                i.vpn_speed
            FROM prev p
            JOIN input i ON i.server_id = p.server_id;
            """;

        var conn = (NpgsqlConnection)_db.Database.GetDbConnection();
        if (conn.State != ConnectionState.Open)
            await conn.OpenAsync(ct);

        await using var cmd = conn.CreateCommand();
        cmd.CommandText = sql;

        cmd.Parameters.Add(new NpgsqlParameter("status_disabled", (int)ServerStatus.Disabled));
        cmd.Parameters.Add(new NpgsqlParameter("server_ids", updates.Select(x => x.ServerId).ToArray()));
        cmd.Parameters.Add(new NpgsqlParameter("statuses", updates.Select(x => (int)x.Status).ToArray()));
        cmd.Parameters.Add(new NpgsqlParameter("reachable", updates.Select(x => x.Reachable).ToArray()));
        cmd.Parameters.Add(new NpgsqlParameter("peers_active", updates.Select(x => x.PeersActive).ToArray()));
        cmd.Parameters.Add(new NpgsqlParameter("rx_total", updates.Select(x => x.RxTotal).ToArray()));
        cmd.Parameters.Add(new NpgsqlParameter("tx_total", updates.Select(x => x.TxTotal).ToArray()));
        cmd.Parameters.Add(new NpgsqlParameter("infra_latency", updates.Select(x => x.InfraLatencyMs).ToArray()));
        cmd.Parameters.Add(new NpgsqlParameter("vpn_speed", updates.Select(x => x.VpnSpeedMbps).ToArray()));
        cmd.Parameters.Add(new NpgsqlParameter(
            "updated_at",
            updates.Select(x => DateTime.SpecifyKind(x.UpdatedAtUtc, DateTimeKind.Utc)).ToArray())
        {
            NpgsqlDbType = NpgsqlDbType.Array | NpgsqlDbType.TimestampTz
        });

        var result = new List<ServerInfoUpdateDto>(updates.Count);

        await using var reader = await cmd.ExecuteReaderAsync(ct);
        while (await reader.ReadAsync(ct))
        {
            var serverId = reader.GetGuid(0);
            var oldStatus = (ServerStatus)reader.GetInt32(1);
            var newStatus = (ServerStatus)reader.GetInt32(2);
            var disabledByFail = reader.GetBoolean(3);
            var reachable = reader.GetBoolean(4);
            var peersActive = reader.GetInt32(5);
            int? maxPeers = reader.IsDBNull(6) ? null : reader.GetInt32(6);
            var failures = reader.GetInt32(7);
            var infraLatencyMs = reader.IsDBNull(8) ? 0d : reader.GetDouble(8);
            var vpnSpeedMbps = reader.IsDBNull(9) ? 0d : reader.GetDouble(9);

            result.Add(new ServerInfoUpdateDto(
                ServerId: serverId,
                OldStatus: oldStatus,
                NewStatus: newStatus,
                DisabledByFail: disabledByFail,
                Reachable: reachable,
                PeersActive: peersActive,
                MaxPeers: maxPeers,
                ConsecutiveFailures: failures,
                InfraLatencyMs: infraLatencyMs,
                VpnSpeedMbps: vpnSpeedMbps
            ));
        }

        return result;
    }
}