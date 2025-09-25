using DrakkarVpn.Agent.Application.Abstractions;
using DrakkarVpn.Agent.Application.DTOs;

namespace DrakkarVpn.Agent.Application.Services;

public sealed class HealthService : IHealthService
{
    private readonly IV2RayService _v2RayService;

    public HealthService(IV2RayService v2RayService)
    {
        _v2RayService = v2RayService;
    }

    public async Task<AgentHealthDto> GetHealthAsync(CancellationToken ct)
    {
        try
        {
            var peers = await _v2RayService.GetListPeersAsync(ct);

            return new AgentHealthDto(
                true,
                peers.Count
            );
        }
        catch
        {
            return new AgentHealthDto(
                false,
                0
            );
        }
    }
}