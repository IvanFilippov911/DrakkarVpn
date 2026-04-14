using DrakkarVpn.Core.Api.Modules.Servers.Domain;

namespace DrakkarVpn.Servers.Domain.Outputs;

public sealed record ServerComputedState(
    ServerStatus Status,
    bool Reachable,
    int PeersActive,
    long RxTotal,
    long TxTotal,
    double InfraLatencyMs,
    double VpnSpeedMbps,
    DateTime ObservedAtUtc
);