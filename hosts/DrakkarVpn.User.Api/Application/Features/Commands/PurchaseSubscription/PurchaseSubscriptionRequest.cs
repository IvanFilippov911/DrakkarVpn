using DrakkarVpn.Core.Api.Modules.Idempotency.Application.Abstracts;
using DrakkarVpn.Core.Api.Modules.Subscriptions.Application.Abstractions;
using DrakkarVpn.Shared.Subscriptions;

namespace DrakkarVpn.Core.Api.Application.Features.Commands.PurchaseSubscription;

public sealed record PurchaseSubscriptionRequest(
    long TelegramId,
    Guid TariffId,
    int DevicesCount, 
    Guid RequestId
) : IIdempotentRequest<SubscriptionDto>, ISubscriptionsCommand<SubscriptionDto>
{
    public string ActorKey => $"tg:{TelegramId}";
    public string Action => "purchase-subscription";
}