using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Orchestrator.Application.Features.Commands.PurchaseSubscription;

public sealed record PurchaseSubscriptionRequest(
    long TelegramId,
    Guid TariffId,
    string? Region = null
) : IRequest<Guid>;