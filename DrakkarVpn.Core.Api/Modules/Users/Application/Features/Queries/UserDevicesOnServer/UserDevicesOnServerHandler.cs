using DrakkarVpn.Core.Api.Modules.Peers.Application.Features.Queries.PeersMapByDeviceIdsOnServer;
using DrakkarVpn.Core.Api.Modules.Subscriptions.Application.Features.Queries.GetActiveSubscriptionByUser;
using DrakkarVpn.Core.Api.Modules.Users.Application.Abstractions;
using DrakkarVpn.Shared;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Users.Application.Features.Queries.UserDevicesOnServer;

public sealed class GetUserDevicesOnServerHandler
    : IRequestHandler<UserDevicesOnServerQuery, IReadOnlyList<DeviceListItemDto>>
{
    private readonly IMediator _mediator;
    private readonly IDeviceRepository _devices;

    public GetUserDevicesOnServerHandler(IMediator mediator, IDeviceRepository devices)
    {
        _mediator = mediator;
        _devices = devices;
    }

    public async Task<IReadOnlyList<DeviceListItemDto>> Handle(UserDevicesOnServerQuery q, CancellationToken ct)
    {
        var sub = await _mediator.Send(new GetActiveSubscriptionByUserRequest(q.UserId), ct);
        if (sub is null) return Array.Empty<DeviceListItemDto>();
        
        var devices = await _devices.ListBySubscriptionAsync(sub.Id, ct);
        if (devices.Count == 0) return Array.Empty<DeviceListItemDto>();
        
        var deviceIds = devices.Select(d => d.DeviceId).ToArray();
        var peersMap = await _mediator.Send(
            new PeersMapByDeviceIdsOnServerQuery(q.ServerId, deviceIds), ct);
        
        var list = devices
            .Select(d =>
            {
                peersMap.TryGetValue(d.DeviceId, out var peer);
                return new DeviceListItemDto(
                    DeviceId: d.DeviceId,
                    Name: d.Name,
                    Platform: d.Platform,
                    CreatedAt: d.CreatedAt,
                    LastSeen: d.LastSeen,
                    Status: (short)d.Status,
                    Peer: peer
                );
            })
            .ToList();

        return list;
    }
}