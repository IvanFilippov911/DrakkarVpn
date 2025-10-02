using DrakkarVpn.Core.Api.Modules.Peers.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Peers.Application.DTOs;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Peers.Application.Features.Queries.GetPeersByUser;

public sealed class GetPeersByUserHandler : IRequestHandler<GetPeersByUserRequest, IReadOnlyList<PeerResponseDto>>
{
    private readonly IPeerRepository _peers;

    public GetPeersByUserHandler(IPeerRepository peers) => _peers = peers;

    public async Task<IReadOnlyList<PeerResponseDto>> Handle(GetPeersByUserRequest req, CancellationToken ct)
    {
        var peers = await _peers.GetByUserAsync(req.UserId, ct);
        return peers
            .Select(p => new PeerResponseDto(
                p.Id.Value,
                p.UserId,
                p.ServerId,
                p.AgentPeerUuid.Value,
                p.Status,
                p.ConfigRaw,
                p.CreatedAt,
                p.ExpiresAt
            ))
            .ToList();
    }
}