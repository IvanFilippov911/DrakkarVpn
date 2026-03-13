using DrakkarVpn.Core.Api.Modules.Peers.Domain;

namespace DrakkarVpn.Core.Api.Modules.Peers.Application.DTOs;

public sealed record PeerRow(
    Guid      PeerId,
    Guid     ServerId,
    Guid      UserId,
    PeerStatus Status,
    bool      IsOnline,
    DateTime? LastDataAtUtc,
    double?   SpeedMbps,
    double?   VpnLatencyMs,
    DateTime  CreatedAtUtc
);