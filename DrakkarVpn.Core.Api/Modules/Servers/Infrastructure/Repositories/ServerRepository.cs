using DrakkarVpn.Core.Api.Infrastructure.EF;
using DrakkarVpn.Core.Api.Modules.Servers.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Servers.Domain;
using DrakkarVpn.Core.Api.Modules.Servers.Domain.VO;
using DrakkarVpn.Core.Api.Modules.Servers.Infrastructure.Models;
using DrakkarVpn.Shared;
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

    public async Task DeleteAsync(Server server, CancellationToken ct)
    {
        _db.Servers.Remove(server);
    }
    
    public IQueryable<Server> Query() => _db.Servers.AsNoTracking();
    
    public async Task UpdateBenchmarkAsync(Guid id, BenchmarkResultDto dto, CancellationToken ct)
    {
        var server = await _db.Servers.FindAsync([id], ct);
        if (server is null) return;
        
        var avgSpeedMbps = (dto.DownloadMbps + dto.UploadMbps) / 2.0;

        server.UpdateBenchmark(avgSpeedMbps);
    }
    
    public async Task<IReadOnlyList<ServerForHealthPoll>> GetForHealthPollAsync(CancellationToken ct)
    {
        
        var rows = await _db.Servers
            .AsNoTracking()
            .Where(s => s.Status == ServerStatus.Disabled || s.Status == ServerStatus.Draining)
            .OrderBy(s => s.Id) 
            .Select(s => new
            {
                Id = s.Id.Value,
                Url = s.AgentBaseUrl.ToString()
            })
            .ToListAsync(ct);
        
        return rows
            .Select(r => new ServerForHealthPoll(r.Id, new Uri(r.Url, UriKind.Absolute)))
            .ToList();
    }



}