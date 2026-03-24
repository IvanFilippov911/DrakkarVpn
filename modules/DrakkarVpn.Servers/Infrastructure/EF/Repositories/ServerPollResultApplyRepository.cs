using System.Data;
using DrakkarVpn.Core.Api.Modules.Servers.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Servers.Application.Features.Queries.GetServers;
using DrakkarVpn.Core.Api.Modules.Servers.Application.Features.Queries.GetServers.ServerState;
using DrakkarVpn.Core.Api.Modules.Servers.Domain;
using DrakkarVpn.Core.Api.Modules.Servers.Infrastructure.EF;
using DrakkarVpn.Observability.Application.DTOs;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace DrakkarVpn.Core.Api.Modules.Servers.Infrastructure.Repositories;

public sealed class ServerPollResultApplyRepository : IServerPollResultApplyRepository
{
    private readonly ServerDbContext _db;
    public ServerPollResultApplyRepository(ServerDbContext db) => _db = db;

    public async Task<IReadOnlyList<ServerInfoUpdateDto>> ApplyBatchAsync(
        IReadOnlyCollection<ServerPollResultDto> appliedResults,
        DateTime nowUtc,
        CancellationToken ct)
    {
        if (appliedResults is null) throw new ArgumentNullException(nameof(appliedResults));
        if (appliedResults.Count == 0) return Array.Empty<ServerInfoUpdateDto>();

        nowUtc = DateTime.SpecifyKind(nowUtc, DateTimeKind.Utc);

        const double peersWarn = 0.95;

        var statusEnabled  = (int)ServerStatus.Enabled;
        var statusDraining = (int)ServerStatus.Draining;
        var statusDisabled = (int)ServerStatus.Disabled;

        const string sql = """
            WITH input AS (
                SELECT *
                FROM UNNEST(
                    @server_ids::uuid[],
                    @reachable::bool[],
                    @peers_active::int[],
                    @rx_total::bigint[],
                    @tx_total::bigint[],
                    @infra_latency::double precision[],
                    @vpn_speed::double precision[]
                ) AS r(
                    server_id,
                    reachable,
                    peers_active,
                    rx_total,
                    tx_total,
                    infra_latency,
                    vpn_speed
                )
            ),
            prev AS (
                SELECT
                    s.id AS server_id,
                    s.status AS old_status,
                    s.max_peers,
                    ps.consecutive_failures
                FROM servers s
                JOIN server_poll_states ps ON ps.server_id = s.id
                JOIN input i ON i.server_id = s.id
            ),
            calc AS (
                SELECT
                    p.server_id,
                    p.old_status,
                    p.max_peers,
                    p.consecutive_failures,
                    i.reachable,
                    i.peers_active,
                    i.rx_total,
                    i.tx_total,
                    i.infra_latency,
                    i.vpn_speed,

                    (NOT i.reachable AND p.consecutive_failures >= 3) AS disabled_by_fail,

                    CASE
                        WHEN (NOT i.reachable AND p.consecutive_failures >= 3)
                            THEN @status_disabled
                        ELSE
                            CASE
                                WHEN i.reachable THEN
                                    CASE
                                        WHEN p.old_status = @status_disabled THEN @status_enabled
                                        WHEN p.old_status = @status_draining THEN @status_enabled
                                        ELSE
                                            CASE
                                                WHEN p.max_peers IS NOT NULL
                                                     AND p.max_peers > 0
                                                     AND i.peers_active >= (p.max_peers * @peers_warn)
                                                    THEN @status_draining
                                                ELSE p.old_status
                                            END
                                    END
                                ELSE p.old_status
                            END
                    END AS new_status
                FROM prev p
                JOIN input i ON i.server_id = p.server_id
            ),
            upd AS (
                UPDATE servers s
                SET
                    status = c.new_status,

                    -- Health (owned)
                    health_reachable   = c.reachable,
                    health_peers_active= c.peers_active,
                    health_updated_at  = @now,

                    -- Metrics (owned)
                    metrics_rx_bytes         = c.rx_total,
                    metrics_tx_bytes         = c.tx_total,
                    metrics_infra_latency_ms = c.infra_latency,
                    metrics_vpn_speed_mbps   = c.vpn_speed,
                    metrics_updated_at       = @now

                FROM calc c
                WHERE s.id = c.server_id
                RETURNING
                    c.server_id,
                    c.old_status,
                    c.new_status,
                    c.disabled_by_fail,
                    c.reachable,
                    c.peers_active,
                    c.max_peers,
                    c.consecutive_failures,
                    c.infra_latency,
                    c.vpn_speed
            )
            SELECT
                server_id,
                old_status,
                new_status,
                disabled_by_fail,
                reachable,
                peers_active,
                max_peers,
                consecutive_failures,
                infra_latency,
                vpn_speed
            FROM upd;
            """;

        await using var conn = (NpgsqlConnection)_db.Database.GetDbConnection();
        if (conn.State != ConnectionState.Open)
            await conn.OpenAsync(ct);

        await using var cmd = conn.CreateCommand();
        cmd.CommandText = sql;

        cmd.Parameters.Add(new NpgsqlParameter("now", nowUtc));
        cmd.Parameters.Add(new NpgsqlParameter("peers_warn", peersWarn));
        cmd.Parameters.Add(new NpgsqlParameter("status_enabled", statusEnabled));
        cmd.Parameters.Add(new NpgsqlParameter("status_draining", statusDraining));
        cmd.Parameters.Add(new NpgsqlParameter("status_disabled", statusDisabled));

        cmd.Parameters.Add(new NpgsqlParameter("server_ids", appliedResults.Select(x => x.ServerId).ToArray()));
        cmd.Parameters.Add(new NpgsqlParameter("reachable", appliedResults.Select(x => x.Reachable).ToArray()));
        cmd.Parameters.Add(new NpgsqlParameter("peers_active", appliedResults.Select(x => x.PeersActive).ToArray()));
        cmd.Parameters.Add(new NpgsqlParameter("rx_total", appliedResults.Select(x => x.RxTotal).ToArray()));
        cmd.Parameters.Add(new NpgsqlParameter("tx_total", appliedResults.Select(x => x.TxTotal).ToArray()));
        cmd.Parameters.Add(new NpgsqlParameter("infra_latency", appliedResults.Select(x => x.InfraLatencyMs).ToArray()));
        cmd.Parameters.Add(new NpgsqlParameter("vpn_speed", appliedResults.Select(x => x.VpnSpeedMbps).ToArray()));

        var updates = new List<ServerInfoUpdateDto>(appliedResults.Count);

        await using var reader = await cmd.ExecuteReaderAsync(ct);
        while (await reader.ReadAsync(ct))
        {
            var serverId       = reader.GetGuid(0);
            var oldStatus      = (ServerStatus)reader.GetInt32(1);
            var newStatus      = (ServerStatus)reader.GetInt32(2);
            var disabledByFail = reader.GetBoolean(3);
            var reachable      = reader.GetBoolean(4);
            var peersActive    = reader.GetInt32(5);
            int? maxPeers      = reader.IsDBNull(6) ? null : reader.GetInt32(6);
            var failures       = reader.GetInt32(7);

            var infraLatencyMs = reader.IsDBNull(8) ? 0d : reader.GetDouble(8);
            var vpnSpeedMbps   = reader.IsDBNull(9) ? 0d : reader.GetDouble(9);

            updates.Add(new ServerInfoUpdateDto(
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

        return updates;
    }
}