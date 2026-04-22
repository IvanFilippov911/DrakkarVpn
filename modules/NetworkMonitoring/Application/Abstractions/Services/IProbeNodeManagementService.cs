using NetworkMonitoring.Application.DTOs;

namespace NetworkMonitoring.Application.Abstractions.Services;

public interface IProbeNodeManagementService
{
    Task<bool> UpdateAsync(
        Guid probeNodeId,
        RegisterProbeNodeDto dto,
        CancellationToken ct);

    Task<bool> DeleteAsync(Guid probeNodeId, CancellationToken ct);
}

