using DrakkarVpn.Core.Api.Modules.Servers.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Servers.Domain;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Servers.Application.Features.Commands.DeleteServer;


public sealed class DeleteServerHandler : IRequestHandler<DeleteServerRequest, bool>
{
    private readonly IServerRepository _repo;
    public DeleteServerHandler(IServerRepository repo) => _repo = repo;

    public async Task<bool> Handle(DeleteServerRequest req, CancellationToken ct)
    {
        var server = await _repo.GetAsync(new ServerId(req.ServerId), ct);
        if (server is null)
            return false;

        await _repo.DeleteAsync(server, ct);
        return true;
    }
}