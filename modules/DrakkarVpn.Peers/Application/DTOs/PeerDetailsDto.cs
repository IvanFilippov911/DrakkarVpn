using DrakkarVpn.Core.Api.Modules.Peers.Domain;
using DrakkarVpn.Core.Api.Modules.Servers.Application.Features.Queries.GetServers;

namespace DrakkarVpn.Core.Api.Modules.Peers.Application.DTOs;

public sealed record PeerDetailsDto(
    ServerShortDto Server,

    Guid      PeerId,
    Guid      UserId,
    PeerStatus Status,
    bool      IsOnline,
    DateTime? LastDataAtUtc,
    double?   SpeedMbps,
    double?   VpnLatencyMs,
    DateTime  CreatedAtUtc,

    long      TrafficLast1hBytes,
    long      TrafficLast24hBytes
);