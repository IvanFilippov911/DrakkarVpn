using DrakkarVpn.Agent.Application.Abstractions;
using DrakkarVpn.Shared;

namespace DrakkarVpn.Agent.Application.Services;

public sealed class MetricService : IMetricService
{
    private readonly IV2RayService _v2RayService;
    private readonly INetworkMetricsService _network;

    private const string DefaultInterface = "eth0";
    private const string PingTarget = "188.225.48.48";

    public MetricService(IV2RayService v2RayService, INetworkMetricsService network)
    {
        _v2RayService = v2RayService;
        _network = network;
    }

    public async Task<AgentMetricsDto> GetMetricAsync(CancellationToken ct)
    {
        try
        {
            var peers = await _v2RayService.GetListPeersAsync(ct);
            var (rx, tx) = await _network.GetTrafficAsync(DefaultInterface, ct);
            var speedMbps = _network.CalculateSpeedMbps(DefaultInterface, rx, tx);
            var infraLatency = await _network.MeasureInfraLatencyAsync(ct);

            return new AgentMetricsDto(
                Reachable: true,
                PeersActive: peers.Count,
                TrafficRxBytes: rx,
                TrafficTxBytes: tx,
                InfraLatencyMs: infraLatency,
                VpnSpeedMbps: speedMbps
            );
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[HealthService] Error: {ex.Message}");
            return AgentMetricsDto.Empty;
        }
    }
}