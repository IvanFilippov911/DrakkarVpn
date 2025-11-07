using DrakkarVpn.Core.Api.Infrastructure.EF;
using DrakkarVpn.Core.Api.Modules.Peers.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Peers.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace DrakkarVpn.Core.Api.Modules.Peers.Infrastructure.Repositories;

public sealed class PeerMetricsHistoryRepository : IPeerMetricsHistoryRepository
{
    private readonly AppDbContext _db;

    public PeerMetricsHistoryRepository(AppDbContext db) => _db = db;

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
                row.TotalRxBytes    = item.TotalRxBytes;
                row.TotalTxBytes    = item.TotalTxBytes;
                row.IsOnline        = item.IsOnline;
                row.VpnLatencyMs    = item.VpnLatencyMs;
            }
            else
            {
                _db.PeerMetricsHistory.Add(item);
            }
        }

        await _db.SaveChangesAsync(ct);
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
}