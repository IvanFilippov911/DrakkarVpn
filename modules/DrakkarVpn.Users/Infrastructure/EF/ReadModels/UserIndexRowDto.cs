using DrakkarVpn.Core.Api.Modules.Users.Domain;

namespace DrakkarVpn.Core.Api.Modules.Users.Infrastructure.Repositories.rowDTOs;

public sealed record UserIndexRowDto(
    Guid       UserId,
    long?      TelegramId,
    DateTime   CreatedAtUtc,
    UserStatus Status,
    bool       IsOnline,
    int        DeviceCount,
    DateTime?  LastSeenUtc,
    
    bool       SubscriptionIsActive,
    DateTime?  SubscriptionEndUtc,
    int        SubscriptionMaxDevices,

    long       TrafficLast24hBytes
);