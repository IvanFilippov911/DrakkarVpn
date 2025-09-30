using DrakkarVpn.Core.Api.Modules.Peers.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Peers.Application.DTOs;
using DrakkarVpn.Core.Api.Modules.Servers.Domain;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Peers.Application.Features.Queries.GetPeersByServer;

public sealed class GetPeersByServerHandler : IRequestHandler<GetPeersByServerRequest, IReadOnlyList<PeerResponseDto>>
{
    private readonly IPeerRepository _peers;

    public GetPeersByServerHandler(IPeerRepository peers) => _peers = peers;

    public async Task<IReadOnlyList<PeerResponseDto>> Handle(GetPeersByServerRequest req, CancellationToken ct)
    {
        var peers = await _peers.GetByServerAsync(req.ServerId, ct);
        return peers
            .Select(p => new PeerResponseDto(
                p.Id.Value,
                p.UserId,
                p.ServerId,
                p.Status,
                p.ConfigRaw,
                p.CreatedAt,
                p.ExpiresAt
            ))
            .ToList();
    }
}