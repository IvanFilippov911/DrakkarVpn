using DrakkarVpn.Core.Api.Modules.Subscriptions.Domain;
using DrakkarVpn.Shared.Tariffs;

namespace DrakkarVpn.Core.Api.Modules.Subscriptions.Application.Abstractions;

public interface ISubscriptionActivationCore
{
    Task<Subscription> CreateOrRenewAsync(
        Guid userId,
        TariffDto tariff,
        int maxDevices,
        DateTime nowUtc,
        CancellationToken ct);
}