namespace DrakkarVpn.Core.Api.Modules.Admin.Application.DTOs.Users;

public sealed record AdminUserRealtimeDto(
    bool      IsOnline              = false,
    int       DeviceCount           = 0,
    int       SubscriptionMaxDevices= 0,
    bool      IsSubscriptionActive  = false,
    DateTime? SubscriptionEndUtc    = null,
    long      Traffic24hBytes       = 0,
    DateTime  UpdatedAtUtc          = default,
    DateTime? LastSeenUtc           = null
);