using DrakkarVpn.Agent.Application.DTOs;
using DrakkarVpn.Shared;

namespace DrakkarVpn.Agent.Application.Abstractions;

public interface IMetricService
{
    Task<AgentMetricsDto> GetMetricAsync(CancellationToken ct);
}