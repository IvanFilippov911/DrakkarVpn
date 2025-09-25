using DrakkarVpn.Core.Api.Modules.Peers.Application.Features.Commands.RevokePeer;
using DrakkarVpn.Core.Api.Modules.Peers.Application.Features.Queries.GetExpiredPeers;
using DrakkarVpn.Core.Api.Modules.Peers.Application.Features.Queries.GetPeersByServer;
using DrakkarVpn.Core.Api.Modules.Servers.Application.Features.Queries.GetServers;
using DrakkarVpn.Core.Api.Modules.Servers.Domain;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Orchestrator.Application.Features.Commands.RenewDuePeers;

public sealed class RenewDuePeersHandler : IRequestHandler<RenewDuePeersRequest, int>
{
    private readonly IMediator _mediator;

    public RenewDuePeersHandler(IMediator mediator) => _mediator = mediator;

    public async Task<int> Handle(RenewDuePeersRequest req, CancellationToken ct)
    {
        var expiredPeers = await _mediator.Send(new GetExpiredPeersRequest(DateTime.UtcNow), ct);

        var count = 0;
        foreach (var peer in expiredPeers)
        {
            var ok = await _mediator.Send(new RevokePeerRequest(peer.Id, peer.ServerId), ct);
            if (ok) count++;
        }

        return count;
    }
}