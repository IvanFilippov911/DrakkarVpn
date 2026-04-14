using DrakkarVpn.Core.Api.Modules.Servers.Domain;

namespace DrakkarVpn.Servers.Application.DTOs.ServerState;

public sealed record ServerUpdate(
    Guid ServerId,
    ServerStatus Status,
    bool Reachable,
    int PeersActive,
    long RxTotal,
    long TxTotal,
    double InfraLatencyMs,
    double VpnSpeedMbps,
    DateTime UpdatedAtUtc
);