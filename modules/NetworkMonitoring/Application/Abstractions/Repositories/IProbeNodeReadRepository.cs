using NetworkMonitoring.Domain;

namespace NetworkMonitoring.Application.Abstractions.Repositories;

public interface IProbeNodeReadRepository
{
    Task<IReadOnlyList<ProbeNode>> ListAsync(CancellationToken ct);
}

