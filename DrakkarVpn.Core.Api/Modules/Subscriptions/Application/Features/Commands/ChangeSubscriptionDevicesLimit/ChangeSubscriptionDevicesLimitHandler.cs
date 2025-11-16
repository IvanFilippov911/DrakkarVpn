using DrakkarVpn.Core.Api.Modules.Subscriptions.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Users.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Users.Application.Features.Commands.ChangeSubscriptionDevicesLimit;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Subscriptions.Application.Features.Commands.ChangeSubscriptionDevicesLimit;

public sealed class ChangeSubscriptionDevicesLimitHandler 
    : IRequestHandler<ChangeSubscriptionDevicesLimitCommand>
{
    private readonly ISubscriptionRepository _subs;
    private readonly IDeviceRepository _devices;

    public ChangeSubscriptionDevicesLimitHandler(
        ISubscriptionRepository subs,
        IDeviceRepository devices)
    {
        _subs    = subs;
        _devices = devices;
    }

    public async Task Handle(ChangeSubscriptionDevicesLimitCommand cmd, CancellationToken ct)
    {
        var sub = await _subs.GetByIdAsync(cmd.SubscriptionId, ct)
                  ?? throw new InvalidOperationException("Subscription not found");

        var used = await _devices.CountActiveBySubscriptionAsync(sub.Id, ct);

        if (cmd.NewMaxDevices < used)
            throw new InvalidOperationException(
                $"Cannot set limit {cmd.NewMaxDevices}, already {used} active devices");

        sub.UpdateMaxDevices(cmd.NewMaxDevices);
    }
}