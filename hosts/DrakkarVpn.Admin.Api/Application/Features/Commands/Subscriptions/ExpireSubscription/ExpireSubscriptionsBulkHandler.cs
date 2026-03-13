using DrakkarVpn.Core.Api.Modules.Peers.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Subscriptions.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Subscriptions.Application.DTOs;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DrakkarVpn.Core.Api.Modules.Subscriptions.Application.Features.Commands.ExpireSubscription;


// TEMPORARY:
// Explicit cross-context orchestration with manual SaveChanges.
// Will be migrated to Outbox / Event-driven flow later.
//
// TODO: добавить алертинг на воркер; внедрить стейт таблицу для корректной работы нескольких инстансов.
public sealed class ExpireSubscriptionsBulkHandler
    : IRequestHandler<ExpireSubscriptionsBulkCommand, ExpireSubscriptionsBulkResultDto>
{
    private readonly ISubscriptionRepository _subs;
    private readonly ISubscriptionExpirationService _expiration;
    private readonly IPeerRevocationService _peers;
    private readonly IPeersUnitOfWork _peersUow;
    private readonly ISubscriptionsUnitOfWork _subsUow;
    private readonly ILogger<ExpireSubscriptionsBulkHandler> _log;

    public ExpireSubscriptionsBulkHandler(
        ISubscriptionRepository subs,
        ISubscriptionExpirationService expiration,
        IPeerRevocationService peers,
        IPeersUnitOfWork peersUow,
        ISubscriptionsUnitOfWork subsUow,
        ILogger<ExpireSubscriptionsBulkHandler> log)
    {
        _subs = subs;
        _expiration = expiration;
        _peers = peers;
        _peersUow = peersUow;
        _subsUow = subsUow;
        _log = log;
    }

    public async Task<ExpireSubscriptionsBulkResultDto> Handle(
        ExpireSubscriptionsBulkCommand cmd,
        CancellationToken ct)
    {
        var markerUtc = DateTime.UtcNow;

        var candidateSubIds = NormalizeIds(cmd.SubscriptionIds);
        if (candidateSubIds.Length == 0)
            return ExpireSubscriptionsBulkResultDto.Empty();
        
        var rows = await _subs.GetExistingByIdsAsync(candidateSubIds, ct);
        if (rows.Count == 0)
            return ExpireSubscriptionsBulkResultDto.Empty();

        var userIds = rows
            .Select(x => x.UserId)
            .Where(x => x != Guid.Empty)
            .Distinct()
            .ToArray();

        if (userIds.Length == 0)
            return ExpireSubscriptionsBulkResultDto.Empty();

        // Step 1: changes in context A (Peers) → SaveChangesAsync(Peers)
        var revoke = await _peers.RevokeUsersPeersAsync(userIds, markerUtc, ct);

        if (revoke.SucceededUserIds.Count == 0)
        {
            _log.LogWarning(
                "ExpireSubscriptionsBulk: revoke failed for all users. candidates={Candidates}, users={Users}, failedUsers={FailedUsers}",
                candidateSubIds.Length, userIds.Length, revoke.FailedUserIds.Count);

            return ExpireSubscriptionsBulkResultDto.RevokeFailedAll(
                candidates: candidateSubIds.Length,
                users: userIds.Length,
                revoke: revoke);
        }

        await _peersUow.SaveChangesAsync(ct);

        var revokeSucceededUsers = revoke.SucceededUserIds.ToHashSet();
        
        var toExpireSubIds = rows
            .Where(r => revokeSucceededUsers.Contains(r.UserId))
            .Select(r => r.SubscriptionId)
            .Where(x => x != Guid.Empty)
            .Distinct()
            .ToArray();

        if (toExpireSubIds.Length == 0)
        {
            _log.LogWarning(
                "ExpireSubscriptionsBulk: nothing to expire after revoke. candidates={Candidates}, users={Users}, revokeSucceededUsers={RevokeOk}",
                candidateSubIds.Length, userIds.Length, revoke.SucceededUserIds.Count);

            return ExpireSubscriptionsBulkResultDto.NothingToExpireAfterRevoke(
                candidates: candidateSubIds.Length,
                users: userIds.Length,
                revoke: revoke);
        }

        // Step 2: changes in context B (Subscriptions) → SaveChangesAsync(Subscriptions)
        var expire = await _expiration.ExpireManyExistingAsync(toExpireSubIds, markerUtc, ct);
        await _subsUow.SaveChangesAsync(ct);

        _log.LogInformation(
            "ExpireSubscriptionsBulk finished: candidates={Candidates}, users={Users}, revokeOkUsers={RevokeOkUsers}, revokeFailedUsers={RevokeFailedUsers}, expireOkSubs={ExpireOkSubs}, expireFailedSubs={ExpireFailedSubs}",
            candidateSubIds.Length,
            userIds.Length,
            revoke.SucceededUserIds.Count,
            revoke.FailedUserIds.Count,
            expire.SucceededSubscriptionIds.Count,
            expire.FailedSubscriptionIds.Count
        );

        return ExpireSubscriptionsBulkResultDto.From(
            candidates: candidateSubIds.Length,
            users: userIds.Length,
            revoke: revoke,
            expire: expire);
    }

    private static Guid[] NormalizeIds(IReadOnlyCollection<Guid> ids)
        => ids
            .Where(x => x != Guid.Empty)
            .Distinct()
            .ToArray();
}