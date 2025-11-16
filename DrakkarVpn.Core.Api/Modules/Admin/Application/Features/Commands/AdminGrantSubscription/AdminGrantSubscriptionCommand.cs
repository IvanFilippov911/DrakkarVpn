using DrakkarVpn.Shared.Subscriptions;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Commands.AdminGrantSubscription;

public sealed record AdminGrantSubscriptionCommand(
    Guid UserId,
    Guid TariffId,
    bool MarkUserInternal,
    int? DeviceCount
) : IRequest<SubscriptionDto>;