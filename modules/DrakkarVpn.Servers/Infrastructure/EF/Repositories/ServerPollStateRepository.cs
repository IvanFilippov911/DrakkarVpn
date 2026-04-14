using System.Data;
using DrakkarVpn.Core.Api.Modules.Servers.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Servers.Application.Features.Queries.GetServers.ServerState;
using DrakkarVpn.Core.Api.Modules.Servers.Infrastructure.EF;
using DrakkarVpn.Servers.Application.DTOs.ServerState;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using NpgsqlTypes;

namespace DrakkarVpn.Core.Api.Modules.Servers.Infrastructure.Repositories;

public sealed class ServerPollStateRepository : IServerPollStateRepository
{
    private readonly ServerDbContext _db;

    public ServerPollStateRepository(ServerDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<ServerPollCandidateDto>> AcquireBatchAsync(
    DateTime nowUtc,
    int batchSize,
    TimeSpan leaseDuration,
    TimeSpan stuckTimeout,
    string instanceId,
    CancellationToken ct)
    {
        if (batchSize <= 0)
            return Array.Empty<ServerPollCandidateDto>();

        if (string.IsNullOrWhiteSpace(instanceId))
            throw new ArgumentException("instanceId is required", nameof(instanceId));

        var leaseUntil = nowUtc + leaseDuration;

        const string sql = """
        WITH picked AS (
            SELECT s."ServerId"
            FROM servers.server_poll_states s
            WHERE
                (
                    s."LeaseUntilUtc" IS NULL
                    OR s."LeaseUntilUtc" <= @now
                    OR (
                        s."LastPollStartedUtc" IS NOT NULL
                        AND s."LastPollStartedUtc" <= @stuck_before
                    )
                )
                AND (
                    s."BackoffUntilUtc" IS NULL
                    OR s."BackoffUntilUtc" <= @now
                )
            ORDER BY COALESCE(s."LastPollFinishedUtc", TIMESTAMPTZ 'epoch') ASC
            LIMIT @batch
            FOR UPDATE SKIP LOCKED
        ),
        leased AS (
            UPDATE servers.server_poll_states u
            SET
                "LeaseOwner" = @instance,
                "LeaseUntilUtc" = @lease_until,
                "LastPollStartedUtc" = @now,
                "LastLeaseRenewedUtc" = @now,
                "UpdatedAtUtc" = @now,
                "LastUpdaterInstance" = @instance
            FROM picked p
            WHERE u."ServerId" = p."ServerId"
            RETURNING
                u."ServerId",
                u.xmin,
                u."LastKnownPeersActive",
                u."LastRxTotal",
                u."LastTxTotal"
        )
        SELECT
            l."ServerId",
            l.xmin,
            l."LastKnownPeersActive",
            l."LastRxTotal",
            l."LastTxTotal",
            srv.agent_base_url
        FROM leased l
        INNER JOIN servers.servers srv ON srv.id = l."ServerId";
        """;


        var conn = (NpgsqlConnection)_db.Database.GetDbConnection();
        if (conn.State != ConnectionState.Open)
            await conn.OpenAsync(ct);

        await using var cmd = conn.CreateCommand();
        cmd.CommandText = sql;

        cmd.Parameters.Add(new NpgsqlParameter("now", nowUtc));
        cmd.Parameters.Add(new NpgsqlParameter("lease_until", leaseUntil));
        cmd.Parameters.Add(new NpgsqlParameter("stuck_before", nowUtc - stuckTimeout));
        cmd.Parameters.Add(new NpgsqlParameter("batch", batchSize));
        cmd.Parameters.Add(new NpgsqlParameter("instance", instanceId));

        var list = new List<ServerPollCandidateDto>(Math.Min(batchSize, 256));

        await using var reader = await cmd.ExecuteReaderAsync(ct);
        while (await reader.ReadAsync(ct))
        {
            var serverId = reader.GetGuid(0);
            var xmin = reader.GetFieldValue<uint>(1);
            var peers = reader.IsDBNull(2) ? 0 : reader.GetInt32(2);
            var rx = reader.IsDBNull(3) ? 0L : reader.GetInt64(3);
            var tx = reader.IsDBNull(4) ? 0L : reader.GetInt64(4);
            var agentBaseUrl = reader.GetString(5);

            list.Add(new ServerPollCandidateDto(
                serverId,
                xmin,
                peers,
                rx,
                tx,
                agentBaseUrl));
        }

        return list;
    }

    public async Task<ApplyPollStateResultDto> ApplyResultsAsync(
        IReadOnlyCollection<ServerPollResultDto> results,
        DateTime nowUtc,
        string instanceId,
        CancellationToken ct)
    {
        if (results.Count == 0)
            return new ApplyPollStateResultDto(Array.Empty<Guid>(), Array.Empty<Guid>());

        if (string.IsNullOrWhiteSpace(instanceId))
            throw new ArgumentException("instanceId is required", nameof(instanceId));

        var unique = results
            .Where(x => x.ServerId != Guid.Empty)
            .GroupBy(x => x.ServerId)
            .Select(g => g.Last())
            .ToList();

        if (unique.Count == 0)
            return new ApplyPollStateResultDto(Array.Empty<Guid>(), Array.Empty<Guid>());

        const string sql = """
        WITH input AS (
            SELECT *
            FROM UNNEST(
                @server_ids::uuid[],
                @xmins::xid[],
                @reachable::bool[],
                @peers_active::int[],
                @rx_total::bigint[],
                @tx_total::bigint[],
                @infra_latency::double precision[],
                @http_latency::int[],
                @success::bool[],
                @error_code::text[]
            ) AS r(
                "ServerId",
                xmin,
                reachable,
                peers_active,
                rx_total,
                tx_total,
                infra_latency,
                http_latency,
                success,
                error_code
            )
        ),
        updated AS (
            UPDATE servers.server_poll_states s
            SET
                "LastPollFinishedUtc" = @now,
                "LastPollLatencyMs" = i.http_latency,
                "LastPollSuccess" = i.success,
                "LastPollErrorCode" = i.error_code,
                "LastUpdaterInstance" = @instance,
                "UpdatedAtUtc" = @now,

                "ConsecutiveFailures" =
                    CASE
                        WHEN i.success AND i.reachable THEN 0
                        ELSE s."ConsecutiveFailures" + 1
                    END,

                "BackoffUntilUtc" =
                    CASE
                        WHEN i.success AND i.reachable THEN NULL
                        ELSE @now + (INTERVAL '5 seconds' * POWER(2, LEAST(s."ConsecutiveFailures" + 1, 6)))
                    END,

                "LastReachableUtc" =
                    CASE
                        WHEN i.success AND i.reachable THEN @now
                        ELSE s."LastReachableUtc"
                    END,

                "LastKnownPeersActive" =
                    CASE
                        WHEN i.success AND i.reachable THEN i.peers_active
                        ELSE s."LastKnownPeersActive"
                    END,

                "LastRxDelta" =
                    CASE
                        WHEN i.success THEN GREATEST(i.rx_total - s."LastRxTotal", 0)
                        ELSE 0
                    END,

                "LastTxDelta" =
                    CASE
                        WHEN i.success THEN GREATEST(i.tx_total - s."LastTxTotal", 0)
                        ELSE 0
                    END,

                "LastRxTotal" =
                    CASE
                        WHEN i.success THEN i.rx_total
                        ELSE s."LastRxTotal"
                    END,

                "LastTxTotal" =
                    CASE
                        WHEN i.success THEN i.tx_total
                        ELSE s."LastTxTotal"
                    END,

                "LastTotalsAtUtc" =
                    CASE
                        WHEN i.success THEN @now
                        ELSE s."LastTotalsAtUtc"
                    END,

                "LastDeltaAtUtc" =
                    CASE
                        WHEN i.success THEN @now
                        ELSE s."LastDeltaAtUtc"
                    END,

                "LeaseOwner" = NULL,
                "LeaseUntilUtc" = NULL

            FROM input i
            WHERE s."ServerId" = i."ServerId"
              AND s."LeaseOwner" = @instance
              AND s."LeaseUntilUtc" IS NOT NULL
              AND s."LeaseUntilUtc" > @now
              AND s.xmin = i.xmin

            RETURNING s."ServerId"
        )
        SELECT "ServerId" FROM updated;
        """;

        var conn = (NpgsqlConnection)_db.Database.GetDbConnection();
        if (conn.State != ConnectionState.Open)
            await conn.OpenAsync(ct);

        await using var cmd = conn.CreateCommand();
        cmd.CommandText = sql;

        cmd.Parameters.Add(new NpgsqlParameter("now", nowUtc));
        cmd.Parameters.Add(new NpgsqlParameter("instance", instanceId));
        cmd.Parameters.Add(new NpgsqlParameter("server_ids", unique.Select(x => x.ServerId).ToArray()));
        cmd.Parameters.Add(new NpgsqlParameter("xmins", unique.Select(x => x.Xmin).ToArray())
        {
            NpgsqlDbType = NpgsqlDbType.Xid | NpgsqlDbType.Array
        });
        cmd.Parameters.Add(new NpgsqlParameter("reachable", unique.Select(x => x.Reachable).ToArray()));
        cmd.Parameters.Add(new NpgsqlParameter("peers_active", unique.Select(x => x.PeersActive).ToArray()));
        cmd.Parameters.Add(new NpgsqlParameter("rx_total", unique.Select(x => x.RxTotal).ToArray()));
        cmd.Parameters.Add(new NpgsqlParameter("tx_total", unique.Select(x => x.TxTotal).ToArray()));
        cmd.Parameters.Add(new NpgsqlParameter("infra_latency", unique.Select(x => x.InfraLatencyMs).ToArray()));
        cmd.Parameters.Add(new NpgsqlParameter("http_latency", unique.Select(x => x.HttpLatencyMs).ToArray()));
        cmd.Parameters.Add(new NpgsqlParameter("success", unique.Select(x => x.Success).ToArray()));
        cmd.Parameters.Add(new NpgsqlParameter("error_code", unique.Select(x => x.ErrorCode).ToArray()));

        var applied = new List<Guid>(unique.Count);

        await using var reader = await cmd.ExecuteReaderAsync(ct);
        while (await reader.ReadAsync(ct))
            applied.Add(reader.GetGuid(0));

        var appliedSet = applied.ToHashSet();
        var skipped = unique
            .Select(x => x.ServerId)
            .Where(id => !appliedSet.Contains(id))
            .ToList();

        return new ApplyPollStateResultDto(applied, skipped);
    }
}