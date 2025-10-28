using DrakkarVpn.Shared.Subscriptions;

namespace DrakkarVpn.Core.Api.Modules.Users.Application.DTOs;

public sealed record UserCardDto(
    UserSummaryDto User,
    SubscriptionSummaryDto Subscription
);