using DrakkarVpn.Core.Api.Modules.Servers.Application.Features.Commands.UpdateServerHealth;
using DrakkarVpn.Core.Api.Modules.Servers.Application.Features.Queries.GetServerById;
using DrakkarVpn.Core.Api.Modules.Servers.Domain;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Orchestrator.Application.Features.Commands.EvaluateServerHealth;

public sealed class EvaluateServerHealthHandler 
    : IRequestHandler<EvaluateServerHealthRequest, bool>
{
    private readonly IMediator _mediator;

    public EvaluateServerHealthHandler(IMediator mediator) => _mediator = mediator;

    public async Task<bool> Handle(EvaluateServerHealthRequest req, CancellationToken ct)
    {
        var server = await _mediator.Send(new GetServerByIdRequest(req.ServerId), ct);
        if (server is null) return false;
        
        if (!req.Reachable)
        {
            var newStatus = server.Status == nameof(ServerStatus.Draining)
                ? ServerStatus.Disabled
                : ServerStatus.Draining;

            return await _mediator.Send(
                new UpdateServerHealthRequest(req.ServerId, newStatus, req.Reachable, req.PeersActive), ct
            );
        }
        
        if (req.PeersActive >= server.MaxPeers)
        {
            return await _mediator.Send(
                new UpdateServerHealthRequest(req.ServerId, ServerStatus.Draining, req.Reachable, req.PeersActive), ct
            );
        }
        
        return await _mediator.Send(
            new UpdateServerHealthRequest(req.ServerId, ServerStatus.Enabled, req.Reachable, req.PeersActive), ct
        );
    }
}