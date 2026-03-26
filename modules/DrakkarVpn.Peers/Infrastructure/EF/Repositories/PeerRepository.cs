

using System.Data;
using System.Linq.Expressions;
using DrakkarVpn.Core.Api.Modules.Peers.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Peers.Application.DTOs;
using DrakkarVpn.Core.Api.Modules.Peers.Domain;
using DrakkarVpn.Core.Api.Modules.Peers.Infrastructure.EF;
using DrakkarVpn.Core.Api.Modules.Users.Domain;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace DrakkarVpn.Peers.Infrastructure.EF.Repositories;

public sealed class PeerRepository : IPeerRepository
{
    private readonly PeerDbContext _db;
    public PeerRepository(PeerDbContext db) => _db = db;

    public Task<Peer?> GetByIdAsync(Guid id, CancellationToken ct) =>
        _db.Peers
            .FirstOrDefaultAsync(p => p.Id == id, ct);
    
    public Task<List<Peer>> GetListActiveByServerAsync(Guid serverId, CancellationToken ct) =>
        _db.Peers
            .Where(p => p.ServerId == serverId && p.Status == PeerStatus.Active)
            .ToListAsync(ct);

    public async Task AddAsync(Peer peer, CancellationToken ct) =>
        await _db.Peers.AddAsync(peer, ct);
    

    public async Task<IReadOnlyList<Peer>> GetByServerAsync(Guid serverId, CancellationToken ct) =>
        await _db.Peers
            .Where(p => p.ServerId == serverId)
            .ToListAsync(ct);
    
    
    public async Task<int> GetActiveCountByServerIdAsync(Guid serverId, CancellationToken ct) =>
        await _db.Peers
            .AsNoTracking()
            .CountAsync(p => 
                    p.ServerId == serverId && 
                    p.Status == PeerStatus.Active,
                ct);
    
    public void Remove(Peer peer)
    {
        _db.Peers.Remove(peer);
    }
    
    public async Task<Peer?> GetByAgentUuidAsync(Guid uuid, CancellationToken ct) =>
        await _db.Peers
            .FirstOrDefaultAsync(p => p.AgentPeerUuid == uuid, ct);

    public Task<Peer?> GetPeerForConfigByAgentUuidAsync(
        Guid agentUuid,
        CancellationToken ct)
        => _db.Peers.AsNoTracking()
            .FirstOrDefaultAsync(p => p.AgentPeerUuid == agentUuid, ct);
    
    public Task<int> SaveChangesAsync(CancellationToken ct) =>
        _db.SaveChangesAsync(ct);
    
    public Task<Peer?> GetByDeviceIdAsync(string deviceId, CancellationToken ct) =>
        _db.Peers.AsNoTracking()
            .FirstOrDefaultAsync(p => p.DeviceId == deviceId && p.Status == PeerStatus.Active, ct);
    
    public async Task<IReadOnlyList<T>> GetForRevokeByUserAsync<T>(
        Guid userId,
        Expression<Func<Peer, T>> selector,
        CancellationToken ct)
    {
        var items = await _db.Devices.AsNoTracking()
            .Where(d => d.UserId == userId)
            .Join(_db.Peers.AsNoTracking(),
                d => d.DeviceId,
                p => p.DeviceId,
                (_, p) => p)
            .Select(selector)
            .ToListAsync(ct);

        return items;
    }
    
    
    
    public Task<int> CountOnServerAsync(Guid serverId, CancellationToken ct)
    {
        return _db.Peers
            .AsNoTracking()
            .CountAsync(p => p.ServerId == serverId, ct);
    }
    
    public async Task<IReadOnlyList<Peer>> GetListActiveByUserAsync(Guid userId, CancellationToken ct)
    {
        var q =
            from p in _db.Peers.AsNoTracking()
            join d in _db.Devices.AsNoTracking()
                on p.DeviceId equals d.DeviceId
            where d.UserId == userId
                  && p.Status == PeerStatus.Active
            select p;

        return await q.ToListAsync(ct);
    }
    
    public async Task<(IReadOnlyList<PeerRow> Items, int Total)> GetServerPeersAsync(
        Guid serverId,
        int page,
        int pageSize,
        CancellationToken ct)
    {
        var (skip, take) = ValidatePagination(page, pageSize);

        var q =
            from p in _db.Peers.AsNoTracking()
            join d in _db.Devices.AsNoTracking()
                on p.DeviceId equals d.DeviceId
            where p.ServerId == serverId
            select new { p, d.UserId };
        
        var total = await q.CountAsync(ct);

        var rows = await q
            .Skip(skip)
            .Take(take)
            .Select(x => new PeerRow(
                x.p.Id,
                serverId,
                x.UserId,
                x.p.Status,
                x.p.IsOnline,
                x.p.LastDataAt,
                x.p.SpeedMbps,
                x.p.VpnLatencyMs,
                x.p.CreatedAt
            ))
            .ToListAsync(ct);

        return (rows, total);
    }
    
    public async Task<Dictionary<Guid, int>> GetOnlineCountsByServerAsync(
        Guid[] serverIds,
        CancellationToken ct)
    {
        if (serverIds is null || serverIds.Length == 0)
            return new();

        var rows = await _db.Peers
            .AsNoTracking()
            .Where(p =>
                serverIds.Contains(p.ServerId) &&
                p.Status == PeerStatus.Active &&
                p.IsOnline)
            .GroupBy(p => p.ServerId)
            .Select(g => new
            {
                ServerId = g.Key,
                Count    = g.Count()
            })
            .ToListAsync(ct);

        return rows.ToDictionary(x => x.ServerId, x => x.Count);
    }

    private static (int skip, int take) ValidatePagination(int page, int pageSize)
    {
        page     = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 1, 100);
        return ((page - 1) * pageSize, pageSize);
    }
    
    public async Task<PeerRow?> GetPeerAsync(
        Guid peerId,
        CancellationToken ct)
    {
        var q =
            from p in _db.Peers.AsNoTracking()
            join d in _db.Devices.AsNoTracking()
                on p.DeviceId equals d.DeviceId
            where p.Id == peerId
            select new PeerRow(
                p.Id,
                p.ServerId,
                d.UserId,
                p.Status,
                p.IsOnline,
                p.LastDataAt,
                p.SpeedMbps,
                p.VpnLatencyMs,
                p.CreatedAt
            );

        return await q.SingleOrDefaultAsync(ct);
    }
    
    public async Task<IReadOnlyList<PeerWithUserRow>> GetListActiveByUsersAsync(
        IReadOnlyCollection<Guid> userIds,
        CancellationToken ct)
    {
        return await (
            from d in _db.Devices.AsNoTracking()
            join p in _db.Peers on d.DeviceId equals p.DeviceId
            where userIds.Contains(d.UserId)
                  && p.Status == PeerStatus.Active
            select new PeerWithUserRow(d.UserId, p)
        ).ToListAsync(ct);
    }
    
    public async Task<IReadOnlyList<PeerRevokeCandidateRow>> GetActiveForRevokeByUsersAsync(
        IReadOnlyCollection<Guid> userIds,
        CancellationToken ct)
    {
        var ids = userIds.Distinct().ToArray();
        if (ids.Length == 0)
            return Array.Empty<PeerRevokeCandidateRow>();
        
        var rows = await (
            from d in _db.Devices.AsNoTracking()
            join p in _db.Peers.AsNoTracking()
                on d.DeviceId equals p.DeviceId
            where ids.Contains(d.UserId)
            where d.Status == (short)DeviceStatus.Active 
            where p.Status == PeerStatus.Active
            where p.AgentPeerUuid != Guid.Empty
            select new PeerRevokeCandidateRow(
                d.UserId,
                d.DeviceId,
                p.Id,
                p.ServerId,
                p.AgentPeerUuid
            )
        ).ToListAsync(ct);

        return rows;
    }

    public Task RevokedManyAsync(
        IReadOnlyCollection<Guid> peerIds,
        DateTime markerUtc,
        CancellationToken ct)
    {
        var ids = peerIds.Distinct().ToArray();

        return _db.Peers
            .Where(p => ids.Contains(p.Id))
            .Where(p => p.Status == PeerStatus.Active)
            .ExecuteUpdateAsync(setters => setters
                    .SetProperty(p => p.Status, PeerStatus.Revoked)
                    .SetProperty(p => p.StatusUpdatedAtUtc, markerUtc),
                ct);
    }

    public async Task<IReadOnlyList<Guid>> GetRevokedPeerIdsByMarkerAsync(
        IReadOnlyCollection<Guid> peerIds,
        DateTime markerUtc,
        CancellationToken ct)
    {
        var ids = peerIds.Distinct().ToArray();

        return await _db.Peers.AsNoTracking()
            .Where(p => ids.Contains(p.Id))
            .Where(p => p.StatusUpdatedAtUtc == markerUtc)
            .Where(p => p.Status == PeerStatus.Revoked)
            .Select(p => p.Id)
            .ToListAsync(ct);
    }
    
    public Task<ActivePeerForDeviceDto?> GetActiveForDeviceAsync(string deviceId, CancellationToken ct)
        => _db.Peers.AsNoTracking()
            .Where(p => p.DeviceId == deviceId)
            .Where(p => p.Status == PeerStatus.Active)
            .Select(p => new ActivePeerForDeviceDto(p.Id, p.ServerId))
            .FirstOrDefaultAsync(ct);

    public Task MarkRevokedAsync(Guid peerId, DateTime markerUtc, CancellationToken ct)
        => _db.Peers
            .Where(p => p.Id == peerId)
            .Where(p => p.Status == PeerStatus.Active)
            .ExecuteUpdateAsync(setters => setters
                    .SetProperty(p => p.Status, PeerStatus.Revoked)
                    .SetProperty(p => p.StatusUpdatedAtUtc, markerUtc),
                ct);

    public Task<bool> IsRevokedByMarkerAsync(Guid peerId, DateTime markerUtc, CancellationToken ct)
        => _db.Peers.AsNoTracking()
            .AnyAsync(p =>
                p.Id == peerId &&
                p.Status == PeerStatus.Revoked &&
                p.StatusUpdatedAtUtc == markerUtc, ct);
    
    
    public async Task<IReadOnlyDictionary<string, Guid>> GetPeerIdsByDeviceIdsAsync(
        IReadOnlyCollection<string> deviceIds,
        CancellationToken ct)
    {
        var ids = deviceIds
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(x => x.Trim())
            .Distinct(StringComparer.Ordinal)
            .ToArray();

        if (ids.Length == 0)
            return new Dictionary<string, Guid>(StringComparer.Ordinal);
        
        var rows = await _db.Peers
            .AsNoTracking()
            .Where(p => ids.Contains(p.DeviceId))
            .Select(p => new { p.DeviceId, p.Id })
            .ToListAsync(ct);

        return rows.ToDictionary(
            x => x.DeviceId,
            x => x.Id,
            StringComparer.Ordinal);
    }

    public async Task CreateManyIgnoreConflictsAsync(
    IReadOnlyCollection<Peer> peers,
    CancellationToken ct)
    {
        var ids          = new Guid[peers.Count];
        var serverIds    = new Guid[peers.Count];
        var agentUuids   = new Guid[peers.Count];
        var deviceIds    = new string[peers.Count];
        var statuses     = new short[peers.Count];
        var createdAtUtc = new DateTime[peers.Count];
        var statusAtUtc  = new DateTime[peers.Count];

        var i = 0;
        foreach (var p in peers)
        {
            ids[i]        = p.Id;
            serverIds[i]  = p.ServerId;
            agentUuids[i] = p.AgentPeerUuid;
            deviceIds[i]  = p.DeviceId;
            statuses[i]   = (short)p.Status;
            createdAtUtc[i] = p.CreatedAt;
            statusAtUtc[i]  = p.StatusUpdatedAtUtc;
            i++;
        }

        const string sql = """
            INSERT INTO peers (
                id,
                server_id,
                agent_peer_uuid,
                device_id,
                status,
                created_at,
                status_updated_at_utc
            )
            SELECT *
            FROM UNNEST(
                @ids::uuid[],
                @server_ids::uuid[],
                @agent_peer_uuids::uuid[],
                @device_ids::text[],
                @statuses::smallint[],
                @created_at::timestamptz[],
                @status_at::timestamptz[]
            ) AS t(
                id,
                server_id,
                agent_peer_uuid,
                device_id,
                status,
                created_at,
                status_updated_at_utc
            )
            ON CONFLICT (device_id) DO NOTHING;
            """;

        await using var conn = (NpgsqlConnection)_db.Database.GetDbConnection();
        if (conn.State != ConnectionState.Open)
            await conn.OpenAsync(ct);

        await using var cmd = new NpgsqlCommand(sql, conn);

        cmd.Parameters.AddWithValue("ids", ids);
        cmd.Parameters.AddWithValue("server_ids", serverIds);
        cmd.Parameters.AddWithValue("agent_peer_uuids", agentUuids);
        cmd.Parameters.AddWithValue("device_ids", deviceIds);
        cmd.Parameters.AddWithValue("statuses", statuses);
        cmd.Parameters.AddWithValue("created_at", createdAtUtc);
        cmd.Parameters.AddWithValue("status_at", statusAtUtc);

        await cmd.ExecuteNonQueryAsync(ct);
    }
}