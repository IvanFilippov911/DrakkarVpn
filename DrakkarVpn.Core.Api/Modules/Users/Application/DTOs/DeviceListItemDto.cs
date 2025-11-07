using DrakkarVpn.Core.Api.Modules.Users.Domain;
using DrakkarVpn.Shared.Peers;

namespace DrakkarVpn.Shared;

public sealed record DeviceListItemDto(
    string DeviceId,
    string? Name,
    string? Platform,
    DateTime CreatedAt,
    DeviceStatus Status,
    PeerBriefDto? Peer 
);