using DrakkarVpn.Core.Api.Modules.Idempotency.Application.Abstracts;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Orchestrator.Application.Features.Commands.PurchaseSubscription;

public sealed record PurchaseSubscriptionRequest(
    long TelegramId,
    Guid TariffId,
    int DevicesCount, 
    Guid RequestId
) : IIdempotentRequest<Guid>
{
    public string ActorKey => $"tg:{TelegramId}";
    public string Action => "purchase-subscription";
}