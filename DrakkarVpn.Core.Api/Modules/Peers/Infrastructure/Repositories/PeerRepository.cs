using DrakkarVpn.Core.Api.Infrastructure.EF;
using DrakkarVpn.Core.Api.Modules.Peers.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Peers.Domain;
using Microsoft.EntityFrameworkCore;

namespace DrakkarVpn.Core.Api.Modules.Peers.Infrastructure.Repositories;

public sealed class PeerRepository : IPeerRepository
{
    private readonly AppDbContext _db;
    public PeerRepository(AppDbContext db) => _db = db;

    public Task<Peer?> GetByIdAsync(PeerId id, CancellationToken ct) =>
        _db.Peers
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id, ct);

    public async Task AddAsync(Peer peer, CancellationToken ct) =>
        await _db.Peers.AddAsync(peer, ct);

    public async Task<IReadOnlyList<Peer>> GetByUserAsync(Guid userId, CancellationToken ct) =>
        await _db.Peers
            .AsNoTracking()
            .Where(p => p.UserId == userId)
            .ToListAsync(ct);

    public async Task<IReadOnlyList<Peer>> GetByServerAsync(Guid serverId, CancellationToken ct) =>
        await _db.Peers
            .AsNoTracking()
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
                    p.Status == PeerStatus.Active && 
                    (p.ExpiresAt == null || p.ExpiresAt > DateTime.UtcNow),
                ct);
}