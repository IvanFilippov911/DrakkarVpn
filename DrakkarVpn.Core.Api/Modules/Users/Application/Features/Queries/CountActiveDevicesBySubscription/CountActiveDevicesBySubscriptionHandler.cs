using DrakkarVpn.Core.Api.Modules.Users.Application.Abstractions;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Users.Application.Features.Queries.CountActiveDevicesBySubscription;

public sealed class CountActiveDevicesBySubscriptionHandler
    : IRequestHandler<CountActiveDevicesBySubscriptionRequest, int>
{
    private readonly IDeviceRepository _devices;
    public CountActiveDevicesBySubscriptionHandler(IDeviceRepository devices) => _devices = devices;

    public Task<int> Handle(CountActiveDevicesBySubscriptionRequest req, CancellationToken ct) =>
        _devices.CountActiveBySubscriptionAsync(req.SubscriptionId, ct);
}