using DrakkarVpn.Core.Api.Modules.Subscriptions.Application.Abstractions;
using DrakkarVpn.Shared.Subscriptions;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Commands.Subscriptions.AdminGrantSubscription;

public sealed class AdminGrantSubscriptionHandler
    : IRequestHandler<AdminGrantSubscriptionCommand, SubscriptionDto>
{
    private readonly ISubscriptionGrantService _svc;

    public AdminGrantSubscriptionHandler(ISubscriptionGrantService svc)
        => _svc = svc;

    public Task<SubscriptionDto> Handle(AdminGrantSubscriptionCommand cmd, CancellationToken ct)
        => _svc.GrantAsync(cmd.UserId, cmd.TariffId, cmd.DeviceCount, DateTime.UtcNow, ct);
}