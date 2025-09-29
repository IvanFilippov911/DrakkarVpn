using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Subscriptions.Application.Features.Commands.ExpireSubscription;

public sealed record ExpireSubscriptionRequest(Guid SubscriptionId) : IRequest<Unit>;