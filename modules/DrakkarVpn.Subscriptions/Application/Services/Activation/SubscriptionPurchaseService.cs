using DrakkarVpn.Core.Api.Modules.Subscriptions.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Subscriptions.Domain;
using DrakkarVpn.Core.Api.Modules.Tariffs.Application.Abstracts;
using DrakkarVpn.Core.Api.Modules.Tariffs.Domain;
using DrakkarVpn.Shared.Subscriptions;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Subscriptions.Application.Services;

public sealed class SubscriptionPurchaseService : ISubscriptionPurchaseService
{
    private readonly ITariffQueryService _tariffs;
    private readonly ISubscriptionActivationCore _core;

    public SubscriptionPurchaseService(
        ITariffQueryService tariffs,
        ISubscriptionActivationCore core)
    {
        _tariffs = tariffs;
        _core    = core;
    }

    public async Task<SubscriptionDto> PurchaseAsync(
        Guid userId,
        Guid tariffId,
        DateTime nowUtc,
        CancellationToken ct)
    {
        var tariff = await _tariffs.GetByIdAsync(tariffId, ct);
        if (tariff is null || tariff.Status != TariffStatus.Active)
            throw new InvalidOperationException("Tariff not available");
        
        var sub = await _core.CreateOrRenewAsync(
            userId,
            tariff,
            tariff.DefaultMaxDevices,
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
}