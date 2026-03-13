using DrakkarVpn.Core.Api.Modules.Subscriptions.Application.DTOs;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Subscriptions.Application.Features.Commands.ExpireSubscription;

public sealed record ExpireSubscriptionsBulkCommand(
    IReadOnlyList<Guid> SubscriptionIds
) : IRequest<ExpireSubscriptionsBulkResultDto>;