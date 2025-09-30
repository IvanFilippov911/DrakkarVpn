using DrakkarVpn.Core.Api.Modules.Peers.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Peers.Application.DTOs;
using DrakkarVpn.Core.Api.Modules.Peers.Domain;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Peers.Application.Features.Queries.GetExpiredPeers;

public sealed class GetExpiredPeersHandler 
    : IRequestHandler<GetExpiredPeersRequest, IReadOnlyList<PeerResponseDto>>
{
    private readonly IPeerRepository _repo;

    public GetExpiredPeersHandler(IPeerRepository repo) => _repo = repo;

    public async Task<IReadOnlyList<PeerResponseDto>> Handle(GetExpiredPeersRequest req, CancellationToken ct)
    {
        var expiredPeers = await _repo.GetExpiredAsync(req.Until, ct);
        return expiredPeers.Select(p => new PeerResponseDto(
            p.Id.Value,
            p.UserId,
            p.ServerId,
            p.Status,
            p.ConfigRaw,
            p.CreatedAt,
            p.ExpiresAt
        )).ToList();
    }
}