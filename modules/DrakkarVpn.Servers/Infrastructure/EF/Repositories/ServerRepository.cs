using DrakkarVpn.Core.Api.Modules.Servers.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Servers.Application.Features.Queries.GetServers;
using DrakkarVpn.Core.Api.Modules.Servers.Domain;
using DrakkarVpn.Core.Api.Modules.Servers.Domain.VO;
using DrakkarVpn.Core.Api.Modules.Servers.Infrastructure.EF;
using DrakkarVpn.Core.Api.Modules.Servers.Infrastructure.EF.ReadModels;
using DrakkarVpn.Servers.Infrastructure.EF.Extensions;
using DrakkarVpn.Shared;
using Microsoft.EntityFrameworkCore;

namespace DrakkarVpn.Core.Api.Modules.Servers.Infrastructure.Repositories;

public sealed class ServerRepository : IServerRepository
{
    private readonly ServerDbContext _db;

    public ServerRepository(ServerDbContext db) => _db = db;

    public Task<Server?> GetAsync(Guid id, CancellationToken ct) =>
        _db.Servers.FirstOrDefaultAsync(s => s.Id == id, ct);
    
    public async Task AddAsync(Server server, CancellationToken ct)
    {
        await _db.Servers.AddAsync(server, ct);
    }

    public async Task DeleteAsync(Server server, CancellationToken ct)
    {
        _db.Servers.Remove(server);
    }
    
    public IQueryable<Server> Query() => _db.Servers.AsNoTracking();
    
    public async Task<Dictionary<Guid, Server>> GetByIdsAsync(Guid[] ids, CancellationToken ct)
    {
        if (ids.Length == 0)
            return new();

        return await _db.Servers
            .Where(s => ids.Contains(s.Id))
            .ToDictionaryAsync(s => s.Id, ct);
    }

    public async Task<(IReadOnlyList<AdminServerIndexRowDto> Items, int Total)> GetPagedAsync(
        string? region,
        ServerStatus? status,
        int page,
        int pageSize,
        CancellationToken ct)
    {
        page = page <= 0 ? 1 : page;
        pageSize = Math.Clamp(pageSize <= 0 ? 50 : pageSize, 1, 200);

        var servers = _db.Servers
            .AsNoTracking()
            .FilterByRegion(region)
            .FilterByStatus(status);

        var total = await servers.CountAsync(ct);
        if (total == 0)
            return (Array.Empty<AdminServerIndexRowDto>(), 0);

        var items = await servers
            .WithRealtimeStats(_db.ServerRealtimeStats.AsNoTracking())
            .OrderByDescending(x => x.Server.Health.Reachable)
            .ThenBy(x => x.Server.Health.PeersActive)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ProjectToAdminIndex()
            .ToListAsync(ct);

        return (items, total);
    }
    
    public async Task<ServerForAgentDto?> GetForAgentAsync(Guid serverId, CancellationToken ct)
    {
        return await _db.Servers
            .AsNoTracking()
            .Where(s => s.Id == serverId)
            .Select(s => new ServerForAgentDto(
                s.Id,
                s.AgentBaseUrl
            ))
            .FirstOrDefaultAsync(ct);
    }
    
    public Task<ServerShortDto?> GetServerShortAsync(Guid serverId, CancellationToken ct)
    {
        return _db.Servers
            .AsNoTracking()
            .Where(s => s.Id == serverId)
            .Select(s => new ServerShortDto(
                s.Id,
                s.Name,
                s.Region.Code,
                s.Status,      
                s.Health.Reachable
            ))
            .SingleOrDefaultAsync(ct);
    }
    
    public Task<string?> GetAgentBaseUrlAsync(Guid serverId, CancellationToken ct)
    {
        return _db.Servers
            .AsNoTracking()
            .Where(s => s.Id == serverId)
            .Select(s => s.AgentBaseUrl.ToString())
            .SingleOrDefaultAsync(ct);
    }
    

}