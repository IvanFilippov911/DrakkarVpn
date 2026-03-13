using DrakkarVpn.Core.Api.Modules.Peers.Application.DTOs;

namespace DrakkarVpn.Core.Api.Modules.Subscriptions.Application.DTOs;

public sealed record ExpireSubscriptionsBulkResultDto(
    int Candidates,
    int Users,
    IReadOnlyList<Guid> RevokeSucceededUserIds,
    IReadOnlyList<Guid> RevokeFailedUserIds,
    IReadOnlyList<Guid> ExpireSucceededSubscriptionIds,
    IReadOnlyList<Guid> ExpireFailedSubscriptionIds)
{
    public static ExpireSubscriptionsBulkResultDto Empty()
        => new(
            Candidates: 0,
            Users: 0,
            RevokeSucceededUserIds: Array.Empty<Guid>(),
            RevokeFailedUserIds: Array.Empty<Guid>(),
            ExpireSucceededSubscriptionIds: Array.Empty<Guid>(),
            ExpireFailedSubscriptionIds: Array.Empty<Guid>());

    public static ExpireSubscriptionsBulkResultDto RevokeFailedAll(int candidates, int users, BulkPeersRevokeResultDto revoke)
        => new(
            Candidates: candidates,
            Users: users,
            RevokeSucceededUserIds: revoke.SucceededUserIds,
            RevokeFailedUserIds: revoke.FailedUserIds,
            ExpireSucceededSubscriptionIds: Array.Empty<Guid>(),
            ExpireFailedSubscriptionIds: Array.Empty<Guid>());

    public static ExpireSubscriptionsBulkResultDto NothingToExpireAfterRevoke(int candidates, int users, BulkPeersRevokeResultDto revoke)
        => new(
            Candidates: candidates,
            Users: users,
            RevokeSucceededUserIds: revoke.SucceededUserIds,
            RevokeFailedUserIds: revoke.FailedUserIds,
            ExpireSucceededSubscriptionIds: Array.Empty<Guid>(),
            ExpireFailedSubscriptionIds: Array.Empty<Guid>());

    public static ExpireSubscriptionsBulkResultDto From(
        int candidates,
        int users,
        BulkPeersRevokeResultDto revoke,
        ExpireSubscriptionsResultDto expire)
        => new(
            Candidates: candidates,
            Users: users,
            RevokeSucceededUserIds: revoke.SucceededUserIds,
            RevokeFailedUserIds: revoke.FailedUserIds,
            ExpireSucceededSubscriptionIds: expire.SucceededSubscriptionIds,
            ExpireFailedSubscriptionIds: expire.FailedSubscriptionIds);
}