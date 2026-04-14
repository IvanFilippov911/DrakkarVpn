namespace DrakkarVpn.Servers.Domain.Inputs;

public sealed record ServerPollResult(
    bool Reachable,
    int PeersActive,
    long RxTotal,
    long TxTotal,
    double InfraLatencyMs,
    double VpnSpeedMbps,
    DateTime ObservedAtUtc,
    int ConsecutiveFailures
);