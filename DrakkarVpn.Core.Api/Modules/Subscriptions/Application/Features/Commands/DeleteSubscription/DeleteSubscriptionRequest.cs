using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Subscriptions.Application.Features.Commands.DeleteSubscription;

public sealed record DeleteSubscriptionRequest(Guid SubscriptionId) : IRequest<bool>;