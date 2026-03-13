namespace DrakkarVpn.Users.Application.DTOs;

public sealed record UserRealtimeDto
(
    bool      IsOnline              = false,
    int       DeviceCount           = 0,
    int       SubscriptionMaxDevices= 0,
    bool      IsSubscriptionActive  = false,
    DateTime? SubscriptionEndUtc    = null,
    long      Traffic24hBytes       = 0,
    DateTime  UpdatedAtUtc          = default,
    DateTime? LastSeenUtc           = null
);