using DrakkarVpn.Core.Api.Modules.Peers.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Peers.Application.DTOs;
using DrakkarVpn.Core.Api.Modules.Peers.Domain;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Peers.Application.Features.Queries.GetPeerById;

public sealed class GetPeerByIdHandler : IRequestHandler<GetPeerByIdRequest, PeerResponseDto>
{
    private readonly IPeerRepository _peers;

    public GetPeerByIdHandler(IPeerRepository peers) => _peers = peers;

    public async Task<PeerResponseDto> Handle(GetPeerByIdRequest req, CancellationToken ct)
    {
        var peer = await _peers.GetByIdAsync(req.PeerId, ct);
        if (peer is null)
            throw new InvalidOperationException($"Peer {req.PeerId} not found");

        return new PeerResponseDto(
            peer.Id,
            peer.DeviceId,
            peer.ServerId,
            peer.AgentPeerUuid,
            peer.Status,
            peer.ConfigRaw,
            peer.CreatedAt
        );
    }
}