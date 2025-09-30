using DrakkarVpn.Shared.Subscriptions;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Subscriptions.Application.Features.Commands.RenewSubscription;

public sealed record RenewSubscriptionRequest(Guid SubscriptionId) : IRequest<SubscriptionDto>;
