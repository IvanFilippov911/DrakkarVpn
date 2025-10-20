using DrakkarVpn.Core.Api.Modules.Users.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Users.Domain.ValueObjects;
using DrakkarVpn.Core.Api.Modules.Peers.Application.Features.Queries.GetPeersByUser;
using DrakkarVpn.Core.Api.Modules.Orchestrator.Application.DTOs;
using DrakkarVpn.Core.Api.Modules.Peers.Application.DTOs;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Orchestrator.Application.Features.Queries.GetUserPeers;

public sealed class GetUserPeersHandler 
    : IRequestHandler<GetUserPeersRequest, IReadOnlyList<GetTgPeersDto>>
{
    private readonly IMediator _mediator;
    private readonly IAppUserRepository _users;

    public GetUserPeersHandler(IMediator mediator, IAppUserRepository users)
    {
        _mediator = mediator;
        _users = users;
    }

    public async Task<IReadOnlyList<GetTgPeersDto>> Handle(GetUserPeersRequest req, CancellationToken ct)
    {
        var tgId = (TelegramId)req.TelegramId;
        var user = await _users.GetByTelegramIdAsync(tgId, ct);

        if (user is null)
            return Array.Empty<GetTgPeersDto>();

        var peers = await _mediator.Send(new GetPeersByUserRequest(user.Id), ct);

        return peers
            .Select(p => new GetTgPeersDto(
                p.Id,
                p.ConfigRaw,
                p.CreatedAt
            ))
            .ToList();
    }
}