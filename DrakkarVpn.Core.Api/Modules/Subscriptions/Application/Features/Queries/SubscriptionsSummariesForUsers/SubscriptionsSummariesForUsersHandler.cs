using DrakkarVpn.Core.Api.Modules.Subscriptions.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Subscriptions.Application.DTOs;
using DrakkarVpn.Shared.Subscriptions;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Subscriptions.Application.Features.Queries.SubscriptionsSummariesForUsers;

public sealed class SubscriptionsSummariesForUsersHandler
    : IRequestHandler<SubscriptionsSummariesForUsersQuery, Dictionary<Guid, SubscriptionSummaryRow>>
{
    private readonly ISubscriptionRepository _repo;
    public SubscriptionsSummariesForUsersHandler(ISubscriptionRepository repo) => _repo = repo;

    public Task<Dictionary<Guid, SubscriptionSummaryRow>> Handle(
        SubscriptionsSummariesForUsersQuery request, CancellationToken ct)
        => _repo.GetSummariesForUsersAsync(request.UserIds, ct);
}