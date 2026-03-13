using DrakkarVpn.Shared.Subscriptions;

namespace DrakkarVpn.Core.Api.Modules.Subscriptions.Application.Abstractions;

public interface ISubscriptionPurchaseService
{
    Task<SubscriptionDto> PurchaseAsync(
        Guid userId,
        Guid tariffId,
        DateTime nowUtc,
        CancellationToken ct);
}