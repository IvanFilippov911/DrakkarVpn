using DrakkarVpn.Core.Api.Infrastructure.EF;
using DrakkarVpn.Core.Api.Modules.Servers.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Servers.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace DrakkarVpn.Core.Api.Modules.Servers.Infrastructure.Repositories;

public sealed class ServerMetricsHistoryRepository : IServerMetricsHistoryRepository
{
    private readonly AppDbContext _db;
    public ServerMetricsHistoryRepository(AppDbContext db) => _db = db;

    public async Task AddOrUpdateMinuteAsync(ServerMetricsHistory e, CancellationToken ct)
    {
        var existing = await _db.ServerMetricsHistory
            .FindAsync(new object?[] { e.PeriodStartUtc, e.ServerId }, ct);

        if (existing is null)
        {
            _db.ServerMetricsHistory.Add(e);
        }
        else
        {
            existing.Reachable      = e.Reachable;
            existing.PeersActive    = e.PeersActive;
            existing.MaxPeers       = e.MaxPeers;
            existing.TrafficRxBytes = e.TrafficRxBytes;
            existing.TrafficTxBytes = e.TrafficTxBytes;
            existing.VpnSpeedMbps   = e.VpnSpeedMbps;
            existing.InfraLatencyMs = e.InfraLatencyMs;
        }
        
    }

    public Task DeleteOlderThanAsync(TimeSpan ttl, CancellationToken ct)
    {
        var border = DateTime.UtcNow - ttl;
        return _db.ServerMetricsHistory
            .Where(x => x.PeriodStartUtc < border)
            .ExecuteDeleteAsync(ct); 
    }
    
    public Task<List<ServerMetricsHistory>> GetRangeAsync(
        Guid serverId,
        DateTime fromUtc,
        DateTime toUtc,
        CancellationToken ct)
    {
        return _db.ServerMetricsHistory
            .AsNoTracking()
            .Where(x => x.ServerId == serverId
                        && x.PeriodStartUtc >= fromUtc
                        && x.PeriodStartUtc <= toUtc)
            .OrderBy(x => x.PeriodStartUtc)
            .ToListAsync(ct);
    }
}
