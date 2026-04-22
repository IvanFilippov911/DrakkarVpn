using NetworkMonitoring.Application.DTOs;

namespace NetworkMonitoring.Application.Abstractions.Services;

public interface IProbeNodesQueryService
{
    Task<IReadOnlyList<ProbeNodeDto>> GetAllAsync(CancellationToken ct);
}

