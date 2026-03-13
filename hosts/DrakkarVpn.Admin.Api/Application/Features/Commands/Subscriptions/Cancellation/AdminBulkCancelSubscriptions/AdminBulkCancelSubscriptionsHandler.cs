using DrakkarVpn.Core.Api.Modules.Peers.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Peers.Application.DTOs;
using DrakkarVpn.Core.Api.Modules.Subscriptions.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Subscriptions.Application.DTOs;
using MediatR;
using BulkCancelSubscriptionsResponse = DrakkarVpn.Core.Api.Modules.Admin.API.Contracts.Subscriptions.Response.BulkCancelSubscriptionsResponse;

namespace DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Commands.Subscriptions.AdminBulkCancelSubscriptions;

// TEMPORARY:
// Explicit cross-context orchestration with manual SaveChanges.
// Will be migrated to Outbox / Event-driven flow later.
public sealed class AdminBulkCancelSubscriptionsHandler
    : IRequestHandler<AdminBulkCancelSubscriptionsCommand, BulkCancelSubscriptionsResponse>
{
    private readonly ISubscriptionRepository _subs;
    private readonly ISubscriptionCancellationService _cancellation;
    private readonly IPeerRevocationService _peers;
    private readonly IPeersUnitOfWork _peersUow;
    private readonly ISubscriptionsUnitOfWork _subsUow;
    private readonly ILogger<AdminBulkCancelSubscriptionsHandler> _log;

    public AdminBulkCancelSubscriptionsHandler(
        ISubscriptionRepository subs,
        ISubscriptionCancellationService cancellation,
        IPeerRevocationService peers,
        IPeersUnitOfWork peersUow,
        ISubscriptionsUnitOfWork subsUow,
        ILogger<AdminBulkCancelSubscriptionsHandler> log)
    {
        _subs = subs;
        _cancellation = cancellation;
        _peers = peers;
        _peersUow = peersUow;
        _subsUow = subsUow;
        _log = log;
    }

    public async Task<BulkCancelSubscriptionsResponse> Handle(
        AdminBulkCancelSubscriptionsCommand cmd,
        CancellationToken ct)
    {
        var markerUtc = DateTime.UtcNow;

        var subscriptionIds = cmd.SubscriptionIds
            .Where(x => x != Guid.Empty)
            .Distinct()
            .ToArray();

        if (subscriptionIds.Length == 0)
            return new BulkCancelSubscriptionsResponse([], []);
        
        var rows = await _subs.GetExistingByIdsAsync(subscriptionIds, ct);
        if (rows.Count == 0)
            return new BulkCancelSubscriptionsResponse([], []);

        var userIds = rows
            .Select(x => x.UserId)
            .Distinct()
            .ToArray();

        // Step 1: changes in context A (Peers) → SaveChangesAsync(Peers)
        var revoke = await _peers.RevokeUsersPeersAsync(userIds, markerUtc, ct);

        if (revoke.SucceededUserIds.Count == 0)
        {
            _log.LogWarning("BulkCancel: all peer revokes failed");

            return new BulkCancelSubscriptionsResponse(
                Succeeded: [],
                Failed: subscriptionIds.ToList(),
                FailureDetails: BuildFailureDetails(rows, revoke));
        }

        await _peersUow.SaveChangesAsync(ct);

        var revokeOkUsers = revoke.SucceededUserIds.ToHashSet();
        
        var toCancel = rows
            .Where(r => revokeOkUsers.Contains(r.UserId))
            .Select(r => r.SubscriptionId)
            .Distinct()
            .ToArray();

        // Step 2: changes in context B (Subscriptions) → SaveChangesAsync(Subscriptions)
        var cancel = await _cancellation.CancelManyAsync(toCancel, markerUtc, ct);
        await _subsUow.SaveChangesAsync(ct);

        return new BulkCancelSubscriptionsResponse(
            Succeeded: cancel.Succeeded,
            Failed: cancel.Failed,
            FailureDetails: revoke.FailedUserIds.Count == 0
                ? null
                : BuildFailureDetails(rows, revoke)
        );
    }

    private static IReadOnlyList<BulkSubscriptionFailureDetail> BuildFailureDetails(
        IReadOnlyList<SubscriptionUserRow> rows,
        BulkPeersRevokeResultDto revoke)
    {
        if (revoke.FailureDetails is null || revoke.FailureDetails.Count == 0)
            return [];

        return revoke.FailureDetails
            .GroupBy(x => x.UserId)
            .SelectMany(g =>
            {
                var subIds = rows
                    .Where(r => r.UserId == g.Key)
                    .Select(r => r.SubscriptionId)
                    .Distinct();

                return subIds.Select(subId => new BulkSubscriptionFailureDetail(
                    SubscriptionId: subId,
                    Code: "PeersRevokeFailed",
                    Message: $"Failed to revoke peers for user {g.Key}",
                    Items: g.Select(f => new BulkSubscriptionFailureItem(
                        UserId: f.UserId,
                        PeerId: f.PeerId,
                        ServerId: f.ServerId,
                        Code: f.Reason
                    )).ToList()
                ));
            })
            .ToList();
    }
}