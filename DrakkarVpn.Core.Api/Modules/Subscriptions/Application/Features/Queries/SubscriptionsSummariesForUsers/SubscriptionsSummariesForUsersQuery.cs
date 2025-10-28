using DrakkarVpn.Core.Api.Modules.Subscriptions.Application.DTOs;
using DrakkarVpn.Shared.Subscriptions;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Subscriptions.Application.Features.Queries.SubscriptionsSummariesForUsers;

public sealed record SubscriptionsSummariesForUsersQuery(
    IReadOnlyCollection<Guid> UserIds
) : IRequest<Dictionary<Guid, SubscriptionSummaryRow>>;