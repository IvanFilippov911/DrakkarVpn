using DrakkarVpn.Core.Api.Modules.Subscriptions.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Subscriptions.Application.DTOs;
using DrakkarVpn.Core.Api.Modules.Subscriptions.Domain;
using DrakkarVpn.Core.Api.Modules.Tariffs.Application.Abstracts;
using DrakkarVpn.Core.Api.Modules.Tariffs.Domain;
using DrakkarVpn.Shared.Subscriptions;

namespace DrakkarVpn.Core.Api.Modules.Subscriptions.Application.Services;

public sealed class SubscriptionGrantService : ISubscriptionGrantService
{
    private readonly ISubscriptionActivationCore   _core;
    private readonly ISubscriptionRepository   _subs;
    private readonly ITariffQueryService       _tariffs;

    public SubscriptionGrantService(
        ISubscriptionActivationCore core,
        ISubscriptionRepository subs,
        ITariffQueryService tariffs)
    {
        _core    = core;
        _subs    = subs;
        _tariffs = tariffs;
    }

    public async Task<SubscriptionDto> GrantAsync(
        Guid userId,
        Guid tariffId,
        int? deviceCount,
        DateTime nowUtc,
        CancellationToken ct)
    {
        var tariff = await _tariffs.GetByIdAsync(tariffId, ct);
        if (tariff is null || tariff.Status != TariffStatus.Active)
            throw new InvalidOperationException("Tariff not available");

        var maxDevices = deviceCount ?? tariff.DefaultMaxDevices;

        var sub = await _core.CreateOrRenewAsync(
            userId,
            tariff,
            maxDevices,
            nowUtc,
            ct);

        return new SubscriptionDto(
            sub.Id,
            sub.UserId,
            sub.StartAt,
            sub.EndAt,
            sub.Status.ToString(),
            sub.MaxDevices);
    }

    public async Task<BulkGrantSubscriptionsResultDto> GrantBulkAsync(
        IReadOnlyCollection<Guid> existingUserIds,
        Guid tariffId,
        int? deviceCount,
        DateTime markerUtc,
        CancellationToken ct)
    {
        var ids = existingUserIds.Where(x => x != Guid.Empty).Distinct().ToList();
        if (ids.Count == 0)
            return BulkGrantSubscriptionsResultDto.Empty();

        var tariff = await _tariffs.GetByIdAsync(tariffId, ct);
        if (tariff is null || tariff.Status != TariffStatus.Active)
            return BulkGrantSubscriptionsResultDto.TariffNotAvailable(ids, tariffId);

        var maxDevices = deviceCount ?? tariff.DefaultMaxDevices;
        var duration   = tariff.Duration;

        var activeByUser = await _subs.GetActiveByUsersAsync(ids, markerUtc, ct);

        var toRenewSubIds   = activeByUser.Values.Select(x => x.SubscriptionId).Distinct().ToList();
        var toCreateUserIds = ids.Where(u => !activeByUser.ContainsKey(u)).ToList();

        if (toRenewSubIds.Count > 0)
            await _subs.RenewManyAsync(toRenewSubIds, markerUtc, duration, maxDevices, markerUtc, ct);

        List<Guid> createdSubIds = [];
        if (toCreateUserIds.Count > 0)
        {
            var rows = toCreateUserIds.Select(u =>
            {
                var id = Guid.NewGuid();
                createdSubIds.Add(id);

                return Subscription.CreateNewForBulk(
                    userId: u,
                    startAtUtc: markerUtc,
                    endAtUtc: markerUtc.Add(duration),
                    maxDevices: maxDevices,
                    markerUtc: markerUtc);
            }).ToList();

            await _subs.CreateManyAsync(rows, ct);
        }

        var succeededSubIds = new HashSet<Guid>();

        if (toRenewSubIds.Count > 0)
        {
            var okRenew = await _subs.GetByStatusMarkerAsync(toRenewSubIds, markerUtc, SubscriptionStatus.Active, ct);
            foreach (var id in okRenew) succeededSubIds.Add(id);
        }

        if (createdSubIds.Count > 0)
        {
            var okCreate = await _subs.GetByStatusMarkerAsync(createdSubIds, markerUtc, SubscriptionStatus.Active, ct);
            foreach (var id in okCreate) succeededSubIds.Add(id);
        }

        var succeededUsers = new HashSet<Guid>();

        foreach (var kv in activeByUser)
            if (succeededSubIds.Contains(kv.Value.SubscriptionId))
                succeededUsers.Add(kv.Key);

        if (createdSubIds.Count > 0)
        {
            var createdUsersOk = await _subs.GetUsersBySubscriptionIdsAsync(createdSubIds, ct);
            foreach (var row in createdUsersOk)
                if (succeededSubIds.Contains(row.SubscriptionId))
                    succeededUsers.Add(row.UserId);
        }

        var succeeded = succeededUsers.ToList();
        var failed    = ids.Where(u => !succeededUsers.Contains(u)).ToList();

        IReadOnlyList<BulkUserGrantFailureDetail>? details = null;
        if (failed.Count > 0)
        {
            details = failed.Select(u => new BulkUserGrantFailureDetail(
                u, "ProvisionFailed", "Subscription was not created/renewed (marker-check failed)"
            )).ToList();
        }

        return BulkGrantSubscriptionsResultDto.Ok(succeeded, failed, details);
    }
}