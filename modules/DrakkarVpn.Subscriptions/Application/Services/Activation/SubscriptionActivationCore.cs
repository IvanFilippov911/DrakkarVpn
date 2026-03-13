using DrakkarVpn.Core.Api.Modules.Subscriptions.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Subscriptions.Domain;
using DrakkarVpn.Core.Api.Modules.Tariffs.Domain;
using DrakkarVpn.Shared.Tariffs;

namespace DrakkarVpn.Core.Api.Modules.Subscriptions.Application.Services.Activation;

public sealed class SubscriptionActivationCore : ISubscriptionActivationCore
{
    private readonly ISubscriptionRepository _subs;

    public SubscriptionActivationCore(ISubscriptionRepository subs)
    {
        _subs = subs;
    }

    public async Task<Subscription> CreateOrRenewAsync(
        Guid userId,
        TariffDto tariff,
        int maxDevices,
        DateTime nowUtc,
        CancellationToken ct)
    {
        var active = await _subs.GetActiveByUserAsync(userId, nowUtc, ct);

        if (active is not null)
        {
            var baseEnd = active.EndAt > nowUtc ? active.EndAt : nowUtc;
            var newEnd  = baseEnd.Add(tariff.Duration);

            active.Renew(newEnd, nowUtc);
            active.UpdateMaxDevices(maxDevices);

            return active;
        }

        var sub = Subscription.CreateNew(
            userId: userId,
            startAtUtc: nowUtc,
            endAtUtc: nowUtc.Add(tariff.Duration),
            maxDevices: maxDevices);

        await _subs.AddAsync(sub, ct);

        return sub;
    }
}