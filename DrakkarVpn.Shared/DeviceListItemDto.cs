using DrakkarVpn.Shared.Peers;

namespace DrakkarVpn.Shared;

public sealed record DeviceListItemDto(
    string DeviceId,
    string? Name,
    string? Platform,
    DateTime CreatedAt,
    DateTime? LastSeen,
    short Status,
    PeerBriefDto? Peer 
);