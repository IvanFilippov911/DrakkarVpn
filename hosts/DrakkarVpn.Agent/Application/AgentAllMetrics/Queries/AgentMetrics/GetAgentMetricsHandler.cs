using DrakkarVpn.Agent.Application.Abstractions;
using DrakkarVpn.Agent.Application.Peers.Queries.Peers;
using DrakkarVpn.Shared;
using MediatR;
using Microsoft.Extensions.Hosting;

namespace DrakkarVpn.Agent.Application.Metrics.Queries.AgentMetrics;

public sealed class GetAgentMetricsHandler
    : IRequestHandler<GetAgentMetricsQuery, AgentMetricsDto>
{
    private readonly IMediator _mediator;
    private readonly INetworkMetricsService _network;
    private readonly IHostEnvironment _hostEnvironment;

    public GetAgentMetricsHandler(
        IMediator mediator,
        INetworkMetricsService network,
        IHostEnvironment hostEnvironment)
    {
        _mediator = mediator;
        _network = network;
        _hostEnvironment = hostEnvironment;
    }

    public async Task<AgentMetricsDto> Handle(GetAgentMetricsQuery request, CancellationToken ct)
    {
        var peers = await _mediator.Send(new PeersQuery(), ct);

        var iface = _hostEnvironment.IsDevelopment() ? "en0" : "eth0";

        var (rx, tx) = await _network.GetTrafficAsync(iface, ct);
        var speedMbps = _network.CalculateSpeedMbps(iface, rx, tx);
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
}