using DrakkarVpn.Core.Api.Application.Features.Commands.PurchaseSubscription;
using DrakkarVpn.Core.Api.Modules.Subscriptions.Application.Abstractions;
using DrakkarVpn.Shared.Subscriptions;
using DrakkarVpn.Users.Application.Abstractions;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Orchestrator.Application.Features.Commands.PurchaseSubscription;

public sealed class PurchaseSubscriptionHandler
    : IRequestHandler<PurchaseSubscriptionRequest, SubscriptionDto>
{
    private readonly IUsersQueryService _usersQuery;
    private readonly ISubscriptionPurchaseService _purchase;

    public PurchaseSubscriptionHandler(
        ISubscriptionPurchaseService purchase,
        IUsersQueryService usersQuery)
    {
        _purchase = purchase;
        _usersQuery = usersQuery;
    }

    public async Task<SubscriptionDto> Handle(PurchaseSubscriptionRequest req, CancellationToken ct)
    {
        var user = await _usersQuery.GetByTelegramIdAsync(req.TelegramId, ct)
                   ?? throw new InvalidOperationException("User not found");

        return await _purchase.PurchaseAsync(
            userId: user.Id,
            tariffId: req.TariffId,
            nowUtc: DateTime.UtcNow,
            ct: ct);
    }
}