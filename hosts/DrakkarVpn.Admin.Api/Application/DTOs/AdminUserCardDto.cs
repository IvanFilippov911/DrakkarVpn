using DrakkarVpn.Core.Api.Modules.Users.Application.DTOs;
using DrakkarVpn.Shared.Subscriptions;

namespace DrakkarVpn.Core.Api.Modules.Admin.Application.DTOs;

public sealed record AdminUserCardDto(
    UserSummaryDto        User,
    SubscriptionSummaryDto Subscription,
    double? TrafficLast24hBytes
);