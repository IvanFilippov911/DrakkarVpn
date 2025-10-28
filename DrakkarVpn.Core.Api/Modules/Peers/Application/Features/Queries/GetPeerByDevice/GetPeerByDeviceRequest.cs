using DrakkarVpn.Shared.Peers;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Peers.Application.Features.Queries.GetPeersBySubscription;

public sealed record GetPeerByDeviceRequest(string DeviceId)
    : IRequest<GetPeerDto>;