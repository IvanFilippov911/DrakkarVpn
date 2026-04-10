using DrakkarVpn.Core.Api.Modules.Orchestrator.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Subscriptions.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Tariffs.Application.Abstracts;
using DrakkarVpn.Core.Api.Modules.Tariffs.Domain;
using DrakkarVpn.Core.Api.Modules.Users.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Users.Domain;

namespace DrakkarVpn.Core.Api.Modules.Orchestrator.Application.Services;

public sealed class GrantTrialSubscriptionService : IGrantTrialSubscriptionService
{
    private readonly IAppUserRepository _users;
    private readonly ITariffQueryService _tariffs;
    private readonly ISubscriptionGrantService _grant;
    private readonly ISubscriptionsUnitOfWork _subscriptionsUow;

    public GrantTrialSubscriptionService(
        IAppUserRepository users,
        ITariffQueryService tariffs,
        ISubscriptionGrantService grant,
        ISubscriptionsUnitOfWork subscriptionsUow)
    {
        _users = users;
        _tariffs = tariffs;
        _grant = grant;
        _subscriptionsUow = subscriptionsUow;
    }

    public async Task TryGrantForNewUserAsync(AppUser user, DateTime nowUtc, CancellationToken ct)
    {
        if (user.TrialGrantedAtUtc is not null)
            return;

        var trialTariff = await _tariffs.GetFirstActiveByKindAsync(TariffKind.Trial, ct);
        if (trialTariff is null)
            return;

        await _grant.GrantAsync(user.Id, trialTariff.Id, null, nowUtc, ct);
        await _subscriptionsUow.SaveChangesAsync(ct);

        user.MarkTrialGranted(nowUtc);
    }
}
