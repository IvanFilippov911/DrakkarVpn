using DrakkarVpn.Core.Api.Infrastructure.EF;
using DrakkarVpn.Core.Api.Modules.Servers.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Servers.Domain;
using Microsoft.EntityFrameworkCore;

namespace DrakkarVpn.Core.Api.Modules.Servers.Infrastructure.Repositories;

public sealed class ServerRepository : IServerRepository
{
    private readonly AppDbContext _db;

    public ServerRepository(AppDbContext db) => _db = db;

    public Task<Server?> GetAsync(ServerId id, CancellationToken ct) =>
        _db.Servers.FirstOrDefaultAsync(s => s.Id == id, ct);
    
    public async Task AddAsync(Server server, CancellationToken ct)
    {
        await _db.Servers.AddAsync(server, ct);
    }
    
    public IQueryable<Server> Query() => _db.Servers.AsQueryable();
}