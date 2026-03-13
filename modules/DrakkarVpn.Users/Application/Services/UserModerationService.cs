using DrakkarVpn.Core.Api.Modules.Users.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Users.Domain;
using DrakkarVpn.Users.Application.Abstractions;
using DrakkarVpn.Users.Application.Abstractions.Servers;
using DrakkarVpn.Users.Application.DTOs.Admin;

namespace DrakkarVpn.Users.Application.Features.Services;

public sealed class UserModerationService : IUserModerationService
{
    private readonly IAppUserRepository _usersRepository;
    private readonly IAppUserReadStore _usersReadStore;

    public UserModerationService(IAppUserRepository users, IAppUserReadStore usersReadStore)
    {
        _usersRepository = users;
        _usersReadStore = usersReadStore;
    }

    public async Task<BulkUsersOperationResult> BanUsersBulkAsync(
        IReadOnlyCollection<Guid> userIds,
        string? reason,
        CancellationToken ct)
    {
        var nowUtc = DateTime.UtcNow;

        var existing = await _usersReadStore.GetExistingIdsAsync(userIds, ct);
        await _usersRepository.BanManyAsync(existing, reason, nowUtc, ct);

        var succeeded = await _usersReadStore.GetByModerationMarkerAsync(
            existing, nowUtc, ModerationMarkerCheck.Banned, ct);

        var succeededSet = succeeded.ToHashSet();
        var failed = existing.Where(id => !succeededSet.Contains(id)).ToList();

        var existingSet = existing.ToHashSet();
        var notFound = userIds.Where(id => !existingSet.Contains(id)).Distinct().ToList();

        return new BulkUsersOperationResult(
            SucceededUserIds: succeeded,
            NotFoundUserIds:  notFound,
            FailedUserIds:    failed
        );
    }

    public async Task<BulkUsersOperationResult> UnbanUsersBulkAsync(
        IReadOnlyCollection<Guid> userIds,
        CancellationToken ct)
    {
        var nowUtc = DateTime.UtcNow;
        var existing = await _usersReadStore.GetExistingIdsAsync(userIds, ct);

        if (existing.Count == 0)
            return new BulkUsersOperationResult(
                SucceededUserIds: [],
                NotFoundUserIds:  userIds.Distinct().ToList(),
                FailedUserIds:    []
            );

        await _usersRepository.UnbanManyAsync(existing, nowUtc, ct);

        var succeeded = await _usersReadStore.GetByModerationMarkerAsync(
            existing, nowUtc, ModerationMarkerCheck.Unbanned, ct);

        var succeededSet = succeeded.ToHashSet();
        var failed = existing.Where(id => !succeededSet.Contains(id)).ToList();

        var existingSet = existing.ToHashSet();
        var notFound = userIds.Where(id => !existingSet.Contains(id)).Distinct().ToList();

        return new BulkUsersOperationResult(
            SucceededUserIds: succeeded,
            NotFoundUserIds:  notFound,
            FailedUserIds:    failed
        );
    }

    public async Task<BulkUsersOperationResult> MarkInternalBulkAsync(
        IReadOnlyCollection<Guid> userIds,
        bool isInternal,
        CancellationToken ct)
    {
        var markerUtc = DateTime.UtcNow;

        var existing = await _usersReadStore.GetExistingIdsAsync(userIds, ct);
        if (existing.Count == 0)
            return new BulkUsersOperationResult(
                SucceededUserIds: [],
                NotFoundUserIds:  userIds.Distinct().ToList(),
                FailedUserIds:    []
            );

        var existingSet = existing.ToHashSet();
        var notFound = userIds.Where(id => !existingSet.Contains(id)).Distinct().ToList();

        await _usersRepository.SetInternalManyAsync(existing, isInternal, markerUtc, ct);

        var check = isInternal ? ModerationMarkerCheck.InternalOn : ModerationMarkerCheck.InternalOff;

        var succeeded = await _usersReadStore.GetByModerationMarkerAsync(existing, markerUtc, check, ct);

        var succeededSet = succeeded.ToHashSet();
        var failed = existing.Where(id => !succeededSet.Contains(id)).ToList();

        return new BulkUsersOperationResult(
            SucceededUserIds: succeeded,
            NotFoundUserIds:  notFound,
            FailedUserIds:    failed
        );
    }
}