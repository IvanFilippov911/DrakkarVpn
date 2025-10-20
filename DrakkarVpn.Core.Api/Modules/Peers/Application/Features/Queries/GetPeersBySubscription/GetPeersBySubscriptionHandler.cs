using DrakkarVpn.Core.Api.Modules.Peers.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Peers.Application.DTOs;
using DrakkarVpn.Core.Api.Modules.Subscriptions.Domain.ValueObjects;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Peers.Application.Features.Queries.GetPeersBySubscription;

public sealed class GetPeersBySubscriptionHandler 
    : IRequestHandler<GetPeersBySubscriptionRequest, IReadOnlyList<PeerResponseDto>>
{
    private readonly IPeerRepository _peers;

    public GetPeersBySubscriptionHandler(IPeerRepository peers) => _peers = peers;

    public async Task<IReadOnlyList<PeerResponseDto>> Handle(GetPeersBySubscriptionRequest req, CancellationToken ct)
    {
        var peers = await _peers.GetBySubscriptionAsync(new SubscriptionId(req.SubscriptionId), ct);

        return peers
            .Select(p => new PeerResponseDto(
                p.Id.Value,
                p.DeviceId,
                p.ServerId,
                p.AgentPeerUuid.Value,
                p.Status,
                p.ConfigRaw,
                p.CreatedAt
            ))
            .ToList();
    }
}