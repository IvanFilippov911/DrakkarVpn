using Microsoft.EntityFrameworkCore;
using NetworkMonitoring.Application.Abstractions.Repositories;
using NetworkMonitoring.Domain;

namespace NetworkMonitoring.Infrastructure.EF.Repositories;

public sealed class ProbeNodeReadRepository : IProbeNodeReadRepository
{
    private readonly NetworkMonitoringDbContext _db;

    public ProbeNodeReadRepository(NetworkMonitoringDbContext db)
        => _db = db;

    public async Task<IReadOnlyList<ProbeNode>> ListAsync(CancellationToken ct)
    {
        var items = await _db.ProbeNodes
            .AsNoTracking()
            .OrderBy(x => x.Region)
            .ThenBy(x => x.Name)
            .ToListAsync(ct);

        return items;
    }
}

