using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Servers.Application.Features.Commands.EvaluateServerHealth;

public sealed record EvaluateServerHealthRequest(
    Guid ServerId,
    bool Reachable,
    int PeersActive,
    long TrafficRxBytes,
    long TrafficTxBytes,
    double InfraLatencyMs,
    double VpnSpeedMbps
) : IRequest<bool>;

