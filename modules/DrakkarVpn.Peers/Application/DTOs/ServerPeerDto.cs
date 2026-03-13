using DrakkarVpn.Core.Api.Modules.Peers.Domain;

namespace DrakkarVpn.Core.Api.Modules.Peers.Application.DTOs;

public sealed record ServerPeerDto(
    Guid      PeerId,
    Guid      UserId,
    PeerStatus   Status,
    bool      IsOnline,
    DateTime? LastDataAtUtc,
    long?      TrafficLast1hBytes,
    long?      TrafficLast24hBytes,
    double?   SpeedMbps,
    double?      VpnLatencyMs,
    DateTime  CreatedAtUtc
);