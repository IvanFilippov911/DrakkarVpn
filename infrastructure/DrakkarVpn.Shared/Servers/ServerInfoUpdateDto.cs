
using DrakkarVpn.Core.Api.Modules.Servers.Domain;

namespace DrakkarVpn.Observability.Application.DTOs;

public sealed record ServerInfoUpdateDto(
    Guid ServerId,
    ServerStatus OldStatus,
    ServerStatus NewStatus,
    bool DisabledByFail,
    bool Reachable,
    int PeersActive,
    int? MaxPeers,
    int ConsecutiveFailures,
    double VpnSpeedMbps,
    double InfraLatencyMs
);