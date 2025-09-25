using DrakkarVpn.Core.Api.Modules.Orchestrator.Application.DTOs;
using DrakkarVpn.Core.Api.Modules.Peers.Application.Features.Queries.GetPeersByUser;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Orchestrator.Application.Features.Queries.GetUserPeers;

public sealed class GetUserPeersHandler 
    : IRequestHandler<GetUserPeersRequest, IReadOnlyList<GetUserPeerDto>>
{
    private readonly IMediator _mediator;

    public GetUserPeersHandler(IMediator mediator) => _mediator = mediator;

    public async Task<IReadOnlyList<GetUserPeerDto>> Handle(GetUserPeersRequest req, CancellationToken ct)
    {
        var peers = await _mediator.Send(new GetPeersByUserRequest(req.UserId), ct);
        
        return peers
            .Select(p => new GetUserPeerDto(
                p.Id,
                p.ServerId,
                p.Status,
                p.ConfigRaw,
                p.CreatedAt,
                p.ExpiresAt
            ))
            .ToList();
    }
}