using DrakkarVpn.Core.Api.Modules.Users.Domain;
using DrakkarVpn.Shared.Subscriptions;

namespace DrakkarVpn.Core.Api.Modules.Users.Infrastructure.Repositories.rowDTOs;

public sealed record UserIndexRowDto(
    Guid       UserId,
    long?      TelegramId,
    DateTime   CreatedAtUtc,
    UserStatus Status,
    bool       IsOnline,
    int        DeviceCount,
    DateTime?  LastSeenUtc,

    SubscriptionStatus? LastSubscriptionStatus,
    DateTime?  SubscriptionEndUtc,
    int        SubscriptionMaxDevices,

    long       TrafficLast24hBytes
);