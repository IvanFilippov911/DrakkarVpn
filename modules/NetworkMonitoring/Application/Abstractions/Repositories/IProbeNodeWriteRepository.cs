using NetworkMonitoring.Domain;

namespace NetworkMonitoring.Application.Abstractions.Repositories;

public interface IProbeNodeWriteRepository
{
    Task<bool> ExistsByNameAsync(string name, CancellationToken ct);
    Task<bool> ExistsByHostAsync(string host, CancellationToken ct);

    Task AddAsync(ProbeNode node, CancellationToken ct);

    Task<ProbeNode?> GetAsync(Guid id, CancellationToken ct);

    Task DeleteAsync(ProbeNode node, CancellationToken ct);
}

