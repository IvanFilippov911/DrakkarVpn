using DrakkarVpn.Core.Api.Modules.Peers.Application.Features.Queries.CountPeersOnServer;
using DrakkarVpn.Core.Api.Modules.Servers.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Servers.Domain;
using DrakkarVpn.Core.Api.Modules.Servers.Domain.VO;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Servers.Application.Features.Commands.DeleteServer;


public sealed class DeleteServerHandler : IRequestHandler<DeleteServerRequest, bool>
{
    private readonly IServerRepository _repo;
    private readonly IMediator _mediator;

    public DeleteServerHandler(IServerRepository repo, IMediator mediator)
    {
        _repo = repo;
        _mediator = mediator;
    }

    public async Task<bool> Handle(DeleteServerRequest req, CancellationToken ct)
    {
        var server = await _repo.GetAsync(req.ServerId, ct);
        if (server is null)
            return false;

        var peersCount = await _mediator.Send(
            new CountPeersOnServerQuery(req.ServerId), ct);

        if (peersCount > 0)
        {
            return false;
        }

        await _repo.DeleteAsync(server, ct);
        return true;
    }
}