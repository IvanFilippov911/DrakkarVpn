using DrakkarVpn.Core.Api.Modules.Peers.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Peers.Infrastructure.EF;

namespace DrakkarVpn.Peers.Infrastructure.EF;

public sealed class PeersUnitOfWork : IPeersUnitOfWork
{
    private readonly PeerDbContext _db;

    public PeersUnitOfWork(PeerDbContext db)
    {
        _db = db;
    }

    public Task<int> SaveChangesAsync(CancellationToken ct = default)
        => _db.SaveChangesAsync(ct);
}