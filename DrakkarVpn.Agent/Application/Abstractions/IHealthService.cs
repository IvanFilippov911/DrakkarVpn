using DrakkarVpn.Agent.Application.DTOs;

namespace DrakkarVpn.Agent.Application.Abstractions;

public interface IHealthService
{
    Task<AgentHealthDto> GetHealthAsync(CancellationToken ct);
}