using DrakkarVpn.Shared.Subscriptions;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Subscriptions.Application.Features.Commands.CreateSubscription;

public sealed record CreateSubscriptionRequest(Guid UserId, Guid TariffId, int DeviceCount) : IRequest<SubscriptionDto>;