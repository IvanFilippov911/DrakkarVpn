using DrakkarVpn.Core.Api.Modules.Users.Domain;
using DrakkarVpn.Users.Application.DTOs.Admin;

namespace DrakkarVpn.Users.Application.DTOs;

public sealed record UserDeviceShortDto(
    string DeviceId,
    string? Name,
    string? Platform,
    DateTime CreatedAtUtc,
    DateTime? LastSeenUtc,
    DeviceStatus Status,
    AdminUserPeerShortDto? Peer
);