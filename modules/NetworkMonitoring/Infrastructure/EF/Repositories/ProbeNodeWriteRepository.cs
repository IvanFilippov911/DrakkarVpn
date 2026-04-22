using Microsoft.EntityFrameworkCore;
using NetworkMonitoring.Application.Abstractions.Repositories;
using NetworkMonitoring.Domain;

namespace NetworkMonitoring.Infrastructure.EF.Repositories;

public sealed class ProbeNodeWriteRepository : IProbeNodeWriteRepository
{
    private readonly NetworkMonitoringDbContext _db;

    public ProbeNodeWriteRepository(NetworkMonitoringDbContext db)
        => _db = db;

    public Task<bool> ExistsByNameAsync(string name, CancellationToken ct)
        => _db.ProbeNodes.AnyAsync(x => x.Name == name, ct);

    public Task<bool> ExistsByHostAsync(string host, CancellationToken ct)
        => _db.ProbeNodes.AnyAsync(x => x.Host == host, ct);

    public Task AddAsync(ProbeNode node, CancellationToken ct)
    {
        _db.ProbeNodes.Add(node);
        return Task.CompletedTask;
    }

    public Task<ProbeNode?> GetAsync(Guid id, CancellationToken ct)
        => _db.ProbeNodes.FirstOrDefaultAsync(x => x.Id == id, ct);

    public Task DeleteAsync(ProbeNode node, CancellationToken ct)
    {
        _db.ProbeNodes.Remove(node);
        return Task.CompletedTask;
    }
}

