using NetworkMonitoring.Application.Abstractions;

namespace NetworkMonitoring.Infrastructure.EF;

public sealed class NetworkMonitoringUnitOfWork : INetworkMonitoringUnitOfWork
{
    private readonly NetworkMonitoringDbContext _db;

    public NetworkMonitoringUnitOfWork(NetworkMonitoringDbContext db)
        => _db = db;

    public Task<int> SaveChangesAsync(CancellationToken ct = default)
        => _db.SaveChangesAsync(ct);
}

