using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Commands.Subscriptions.AdminCancelSubscription;

public sealed record AdminCancelSubscriptionCommand(Guid SubscriptionId) : IRequest<bool>;