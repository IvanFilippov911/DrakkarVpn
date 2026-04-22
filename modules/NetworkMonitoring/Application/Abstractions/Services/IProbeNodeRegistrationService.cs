using NetworkMonitoring.Application.DTOs;

namespace NetworkMonitoring.Application.Abstractions.Services;

public interface IProbeNodeRegistrationService
{
    Task<Guid> RegisterAsync(RegisterProbeNodeDto dto, CancellationToken ct);
}

