namespace NetworkMonitoring.Application.Abstractions;

public interface INetworkMonitoringUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}

