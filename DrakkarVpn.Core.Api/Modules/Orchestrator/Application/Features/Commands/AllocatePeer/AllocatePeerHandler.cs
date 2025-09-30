using DrakkarVpn.Core.Api.Modules.Peers.Application.DTOs;
using DrakkarVpn.Core.Api.Modules.Peers.Application.Features.Commands.CreatePeer;
using DrakkarVpn.Core.Api.Modules.Peers.Application.Features.Queries.GetPeersByUser;
using DrakkarVpn.Core.Api.Modules.Servers.Application.Features.Queries.GetServers;
using DrakkarVpn.Core.Api.Modules.Servers.Domain;
using DrakkarVpn.Core.Api.Modules.Subscriptions.Application.Features.Queries.GetActiveSubscriptionByUser;
using DrakkarVpn.Core.Api.Modules.Users.Application.Features.Queries.GetUserByTelegramId;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Orchestrator.Application.Features.Commands.AllocatePeer;

public sealed class AllocatePeerHandler : IRequestHandler<AllocatePeerRequest, PeerRegisterResponseDto>
{
    private readonly IMediator _mediator;
    private const int MaxPeersPerUser = 2;

    public AllocatePeerHandler(IMediator mediator) => _mediator = mediator;

    public async Task<PeerRegisterResponseDto> Handle(AllocatePeerRequest req, CancellationToken ct)
    {
        var user = await _mediator.Send(new GetUserByTelegramIdRequest(req.TelegramId), ct);
        
        var peers = await _mediator.Send(new GetPeersByUserRequest(user.Id), ct);
        if (peers.Count >= MaxPeersPerUser)
            throw new InvalidOperationException("Peer limit exceeded");
        
        var servers = await _mediator.Send(new GetServersRequest(req.Region, nameof(ServerStatus.Enabled)), ct);
        var server = servers
            .OrderBy(s => s.PeersActive)
            .FirstOrDefault();
        if (server is null)
            throw new InvalidOperationException("No enabled servers available");
        
        var createReq = new RegisterPeerRequest(
            user.Id,
            server.Id,
            req.SubscriptionId,
            req.EndAt
        );

        var peer = await _mediator.Send(createReq, ct);

        return peer;
    }
}