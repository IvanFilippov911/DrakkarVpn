using DrakkarVpn.Core.Api.Modules.Peers.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Users.Application.Abstractions;
using DrakkarVpn.Shared.Peers;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Peers.Application.Features.Queries.GetPeersBySubscription;

public sealed class GetPeerByDeviceHandler : IRequestHandler<GetPeerByDeviceRequest, GetPeerDto>
{
    private readonly IPeerRepository _peers;
    public GetPeerByDeviceHandler(IPeerRepository peers) => _peers = peers;

    public async Task<GetPeerDto?> Handle(GetPeerByDeviceRequest req, CancellationToken ct)
    {
        var peer = await _peers.GetByDeviceIdAsync(req.DeviceId, ct);
        if (peer is null) return null;


        return new GetPeerDto(
            Id: peer.Id,
            ServerId: peer.ServerId,
            DeviceId: peer.DeviceId,
            AgentPeerId: peer.AgentPeerUuid,
            Status: (int)peer.Status,
            CreatedAt: peer.CreatedAt,
            ConfigRaw: peer.ConfigRaw
        );
    }
    
    
    
}