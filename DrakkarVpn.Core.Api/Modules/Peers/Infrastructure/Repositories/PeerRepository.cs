using DrakkarVpn.Core.Api.Infrastructure.EF;
using DrakkarVpn.Core.Api.Modules.Peers.Application.Abstractions;
using System.Linq.Expressions;
using DrakkarVpn.Core.Api.Modules.Peers.Domain;
using DrakkarVpn.Shared.Peers;
using Microsoft.EntityFrameworkCore;

namespace DrakkarVpn.Core.Api.Modules.Peers.Infrastructure.Repositories;

public sealed class PeerRepository : IPeerRepository
{
    private readonly AppDbContext _db;
    public PeerRepository(AppDbContext db) => _db = db;

    public Task<Peer?> GetByIdAsync(Guid id, CancellationToken ct) =>
        _db.Peers
            .FirstOrDefaultAsync(p => p.Id == id, ct);

    public async Task AddAsync(Peer peer, CancellationToken ct) =>
        await _db.Peers.AddAsync(peer, ct);
    

    public async Task<IReadOnlyList<Peer>> GetByServerAsync(Guid serverId, CancellationToken ct) =>
        await _db.Peers
            .Where(p => p.ServerId == serverId)
            .ToListAsync(ct);

    public async Task<IReadOnlyList<Peer>> GetAllAsync(int limit, int offset, CancellationToken ct) =>
        await _db.Peers
            .AsNoTracking()
            .OrderByDescending(p => p.CreatedAt)
            .Skip(offset)
            .Take(limit)
            .ToListAsync(ct);

    public IQueryable<Peer> Query() => _db.Peers.AsQueryable();
    
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
    
    public Task<int> SaveChangesAsync(CancellationToken ct) =>
        _db.SaveChangesAsync(ct);
    
    public Task<Peer?> GetByDeviceIdAsync(string deviceId, CancellationToken ct) =>
        _db.Peers.AsNoTracking()
            .FirstOrDefaultAsync(p => p.DeviceId == deviceId, ct);
    
    public async Task<IReadOnlyList<T>> GetForRevokeAsync<T>(
        Guid subscriptionId,
        Expression<Func<Peer, T>> selector,
        CancellationToken ct)
    {
        var items = await _db.Devices.AsNoTracking()
            .Where(d => d.SubscriptionId == subscriptionId)
            .Join(_db.Peers.AsNoTracking(),
                d => d.DeviceId,
                p => p.DeviceId,
                (_, p) => p)
            .Select(selector)
            .ToListAsync(ct); 

        return items;
    }
    
    public async Task<Dictionary<string, PeerBriefDto>> GetMapByDeviceIdsOnServerAsync(
        Guid serverId,
        IReadOnlyCollection<string> deviceIds,
        CancellationToken ct)
    {
        if (deviceIds is null || deviceIds.Count == 0)
            return new();

        var pairs = await _db.Peers
            .Where(p => p.ServerId == serverId && deviceIds.Contains(p.DeviceId))
            .Select(p => new
            {
                p.DeviceId,
                Peer = new PeerBriefDto(
                    p.Id,
                    p.AgentPeerUuid,
                    (short)p.Status,
                    p.LastDataAt,
                    p.TotalRxBytes,
                    p.TotalTxBytes,
                    p.VpnLatencyMs,
                    p.IsOnline
                )
            })
            .ToListAsync(ct);

        return pairs.ToDictionary(x => x.DeviceId, x => x.Peer);
    }
    
}