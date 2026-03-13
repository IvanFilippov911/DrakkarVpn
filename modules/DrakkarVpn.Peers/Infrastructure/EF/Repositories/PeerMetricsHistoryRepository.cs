using DrakkarVpn.Core.Api.Modules.Admin.Application.DTOs;
using DrakkarVpn.Core.Api.Modules.Peers.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Peers.Application.DTOs;
using DrakkarVpn.Core.Api.Modules.Peers.Infrastructure.EF;
using DrakkarVpn.Core.Api.Modules.Peers.Infrastructure.Entities;
using DrakkarVpn.Core.Api.Modules.Servers.Application.Features.Queries.GetServers;
using Microsoft.EntityFrameworkCore;

namespace DrakkarVpn.Core.Api.Modules.Peers.Infrastructure.Repositories;

public sealed class PeerMetricsHistoryRepository : IPeerMetricsHistoryRepository
{
    private readonly PeerDbContext _db;

    public PeerMetricsHistoryRepository(PeerDbContext db) => _db = db;

    public async Task UpsertRangeAsync(
        IReadOnlyCollection<PeerMetricsHistory> items,
        CancellationToken ct)
    {
        if (items.Count == 0)
            return;

        var period = items.First().PeriodStartUtc;
        var peerIds = items.Select(i => i.PeerId).ToArray();

        var existing = await _db.PeerMetricsHistory
            .Where(x => x.PeriodStartUtc == period && peerIds.Contains(x.PeerId))
            .ToDictionaryAsync(x => x.PeerId, ct);

        foreach (var item in items)
        {
            if (existing.TryGetValue(item.PeerId, out var row))
            {
                row.TotalRxBytes  = item.TotalRxBytes;
                row.TotalTxBytes  = item.TotalTxBytes;
                row.RxDeltaBytes  = item.RxDeltaBytes;
                row.TxDeltaBytes  = item.TxDeltaBytes;
                row.IsOnline      = item.IsOnline;
                row.VpnLatencyMs  = item.VpnLatencyMs;
            }
            else
            {
                _db.PeerMetricsHistory.Add(item);
            }
        }
    }

    public Task DeleteOlderThanAsync(TimeSpan ttl, CancellationToken ct)
    {
        var border = DateTime.UtcNow - ttl;
        return _db.PeerMetricsHistory
            .Where(x => x.PeriodStartUtc < border)
            .ExecuteDeleteAsync(ct);
    }
    
    public async Task<IReadOnlyList<PeerMetricsHistory>> GetRangeAsync(
        Guid peerId,
        DateTime fromUtc,
        DateTime toUtc,
        CancellationToken ct)
    {
        var rows = await _db.PeerMetricsHistory
            .AsNoTracking()
            .Where(x =>
                x.PeerId == peerId &&
                x.PeriodStartUtc >= fromUtc &&
                x.PeriodStartUtc <= toUtc)
            .OrderBy(x => x.PeriodStartUtc)
            .ToListAsync(ct);

        return rows; 
    }
    
    public async Task<IReadOnlyList<ServerOnlinePointDto>> GetServerPeersOnlineTimelineAsync(
        Guid serverId,
        DateTime fromUtc,
        DateTime toUtc,
        CancellationToken ct)
    {
        return await _db.PeerMetricsHistory
            .AsNoTracking()
            .Where(x =>
                x.ServerId == serverId &&
                x.PeriodStartUtc >= fromUtc &&
                x.PeriodStartUtc <= toUtc &&
                x.IsOnline)
            .GroupBy(x => x.PeriodStartUtc)
            .OrderBy(g => g.Key)               
            .Select(g => new ServerOnlinePointDto(
                g.Key,                          
                g.Count()                       
            ))
            .ToListAsync(ct);
    }
    
    public async Task<IReadOnlyList<SuspiciousPeerTrafficDto>> GetRecentTrafficSummaryAsync(
        DateTime fromUtc,
        DateTime toUtc,
        CancellationToken ct)
    {
        var query =
            from h in _db.PeerMetricsHistory
            where h.PeriodStartUtc >= fromUtc && h.PeriodStartUtc < toUtc
            group new { h } by new { h.PeerId, h.ServerId } into g
            select new SuspiciousPeerTrafficDto(
                g.Key.PeerId,
                g.Key.ServerId,
                fromUtc,
                toUtc,
                g.Sum(x => x.h.RxDeltaBytes),
                g.Sum(x => x.h.TxDeltaBytes),
                g.Max(x => x.h.SpeedMbps),
               g.Any(x => x.h.IsOnline)
            );

        return await query.ToListAsync(ct);
    }
    
    public async Task<Dictionary<Guid, long>> GetPeersTrafficBytesOnServerAsync(
        Guid serverId,
        Guid[] peerIds,
        DateTime fromUtc,
        DateTime toUtc,
        CancellationToken ct)
    {
        if (peerIds is null || peerIds.Length == 0)
            return new();

        var rows =
            await _db.PeerMetricsHistory
                .AsNoTracking()
                .Where(h =>
                    h.ServerId == serverId &&
                    peerIds.Contains(h.PeerId) &&
                    h.PeriodStartUtc >= fromUtc &&
                    h.PeriodStartUtc <  toUtc)
                .GroupBy(h => h.PeerId)
                .Select(g => new
                {
                    PeerId        = g.Key,
                    TrafficBytes  = g.Sum(x => x.RxDeltaBytes + x.TxDeltaBytes)
                })
                .ToListAsync(ct);

        return rows.ToDictionary(x => x.PeerId, x => x.TrafficBytes);
    }
    
    public async Task<long> GetPeerTrafficBytesAsync(
        Guid peerId,
        DateTime fromUtc,
        DateTime toUtc,
        CancellationToken ct)
    {
        if (fromUtc >= toUtc)
            return 0;

        var bytes = await _db.PeerMetricsHistory
            .AsNoTracking()
            .Where(h =>
                h.PeerId == peerId &&
                h.PeriodStartUtc >= fromUtc &&
                h.PeriodStartUtc <  toUtc)
            .SumAsync(h => h.RxDeltaBytes + h.TxDeltaBytes, ct);

        return bytes;
    }
    
    public async Task<IReadOnlyList<PeerTrafficWindowRow>> GetTrafficAggForWindowAsync(
        DateTime fromUtc,
        DateTime toUtc,
        CancellationToken ct)
    {
        var rows = await _db.PeerMetricsHistory
            .AsNoTracking()
            .Where(h =>
                h.PeriodStartUtc >= fromUtc &&
                h.PeriodStartUtc <  toUtc)
            .GroupBy(h => h.PeerId)
            .Select(g => new PeerTrafficWindowRow(
                g.Key,
                g.Sum(x => x.RxDeltaBytes + x.TxDeltaBytes)
            ))
            .ToListAsync(ct);

        return rows;
    }

}