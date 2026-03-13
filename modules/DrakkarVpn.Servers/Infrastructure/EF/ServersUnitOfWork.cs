using DrakkarVpn.Core.Api.Modules.Servers.Infrastructure.EF;
using DrakkarVpn.Servers.Application.Abstractions;

namespace DrakkarVpn.Servers.Infrastructure.EF;

public sealed class ServersUnitOfWork : IServersUnitOfWork
{
    private readonly ServerDbContext _db;

    public ServersUnitOfWork(ServerDbContext db)
    {
        _db = db;
    }

    public Task<int> SaveChangesAsync(CancellationToken ct = default)
        => _db.SaveChangesAsync(ct);
}