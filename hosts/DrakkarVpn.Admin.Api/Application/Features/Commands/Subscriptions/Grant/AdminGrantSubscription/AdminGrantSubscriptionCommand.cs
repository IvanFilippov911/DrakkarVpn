using DrakkarVpn.Core.Api.Modules.Subscriptions.Application.Abstractions;
using DrakkarVpn.Shared.Subscriptions;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Commands.Subscriptions.AdminGrantSubscription;

public sealed record AdminGrantSubscriptionCommand(
    Guid UserId,
    Guid TariffId,
    int? DeviceCount
) : IRequest<SubscriptionDto>, ISubscriptionsCommand<SubscriptionDto>;