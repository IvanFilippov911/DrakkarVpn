using DrakkarVpn.Agent.Application.Abstractions;
using DrakkarVpn.Agent.Application.Peers.Queries.Peers;
using DrakkarVpn.Shared;
using MediatR;

namespace DrakkarVpn.Agent.Application.Metrics.Queries.AgentMetrics;

public sealed class GetAgentMetricsHandler
    : IRequestHandler<GetAgentMetricsQuery, AgentMetricsDto>
{
    private readonly IMediator _mediator;
    private readonly INetworkMetricsService _network;

    public GetAgentMetricsHandler(IMediator mediator, INetworkMetricsService network)
    {
        _mediator = mediator;
        _network = network;
    }

    public async Task<AgentMetricsDto> Handle(GetAgentMetricsQuery request, CancellationToken ct)
    {
        var peers = await _mediator.Send(new PeersQuery(), ct);

        var (rx, tx) = await _network.GetTrafficAsync("en0", ct);
        var speedMbps = _network.CalculateSpeedMbps("en0", rx, tx);
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