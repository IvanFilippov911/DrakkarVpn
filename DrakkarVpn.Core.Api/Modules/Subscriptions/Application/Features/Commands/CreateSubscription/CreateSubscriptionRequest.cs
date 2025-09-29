using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Subscriptions.Application.Features.Commands.CreateSubscription;

public sealed record CreateSubscriptionRequest(Guid UserId, Guid TariffId) : IRequest<Guid>;