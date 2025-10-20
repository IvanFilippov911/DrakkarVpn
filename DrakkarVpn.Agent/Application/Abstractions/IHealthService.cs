using DrakkarVpn.Agent.Application.DTOs;
using DrakkarVpn.Shared;

namespace DrakkarVpn.Agent.Application.Abstractions;

public interface IHealthService
{
    Task<AgentMetricsDto> GetHealthAsync(CancellationToken ct);
}