using DrakkarVpn.Core.Api.Modules.Admin.API.Contracts.Subscriptions.Response;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Commands.Subscriptions.AdminBulkCancelSubscriptions;

public sealed record AdminBulkCancelSubscriptionsCommand(
    IReadOnlyCollection<Guid> SubscriptionIds
) : IRequest<BulkCancelSubscriptionsResponse>;