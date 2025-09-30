using DrakkarVpn.Core.Api.Modules.Peers.Application.DTOs;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Peers.Application.Features.Queries.GetPeersBySubscription;

public sealed record GetPeersBySubscriptionRequest(Guid SubscriptionId) 
    : IRequest<IReadOnlyList<PeerResponseDto>>;