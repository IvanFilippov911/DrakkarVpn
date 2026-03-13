using DrakkarVpn.Core.Api.Modules.Peers.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Subscriptions.Application.Abstractions;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Commands.Subscriptions.AdminCancelSubscription;

// TEMPORARY:
// Explicit cross-context orchestration with manual SaveChanges.
// Will be migrated to Outbox / Event-driven flow later.
public sealed class AdminCancelSubscriptionHandler
    : IRequestHandler<AdminCancelSubscriptionCommand, bool>
{
    private readonly ISubscriptionRepository _subs;
    private readonly ISubscriptionCancellationService _cancellation;
    private readonly IPeerRevocationService _peers;
    private readonly IPeersUnitOfWork _peersUow;
    private readonly ISubscriptionsUnitOfWork _subsUow;

    public AdminCancelSubscriptionHandler(
        ISubscriptionRepository subs,
        ISubscriptionCancellationService cancellation,
        IPeerRevocationService peers,
        IPeersUnitOfWork peersUow,
        ISubscriptionsUnitOfWork subsUow)
    {
        _subs = subs;
        _cancellation = cancellation;
        _peers = peers;
        _peersUow = peersUow;
        _subsUow = subsUow;
    }

    public async Task<bool> Handle(
        AdminCancelSubscriptionCommand cmd,
        CancellationToken ct)
    {
        var markerUtc = DateTime.UtcNow;

        var rows = await _subs.GetExistingByIdsAsync([cmd.SubscriptionId], ct);
        if (rows.Count == 0)
            return false;

        var userId = rows[0].UserId;

        // Step 1: changes in context A (Peers) → SaveChangesAsync(Peers)
        var revoke = await _peers.RevokeUsersPeersAsync([userId], markerUtc, ct);

        if (revoke.SucceededUserIds.Count == 0)
            throw new InvalidOperationException("Peer revoke failed");

        await _peersUow.SaveChangesAsync(ct);

        // Step 2: changes in context B (Subscriptions) → SaveChangesAsync(Subscriptions)
        var cancel = await _cancellation.CancelManyAsync(
            [cmd.SubscriptionId],
            markerUtc,
            ct);

        if (cancel.Succeeded.Count == 0)
            throw new InvalidOperationException("Subscription cancel failed");

        await _subsUow.SaveChangesAsync(ct);

        return true;
    }
}