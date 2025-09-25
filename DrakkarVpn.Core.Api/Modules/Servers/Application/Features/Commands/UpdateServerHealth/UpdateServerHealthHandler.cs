using DrakkarVpn.Core.Api.Modules.Servers.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Servers.Domain;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Servers.Application.Features.Commands.UpdateServerHealth;

public sealed class UpdateServerHealthHandler : IRequestHandler<UpdateServerHealthRequest, bool>
{
    private readonly IServerRepository _repo;

    public UpdateServerHealthHandler(IServerRepository repo) => _repo = repo;

    public async Task<bool> Handle(UpdateServerHealthRequest req, CancellationToken ct)
    {
        var server = await _repo.GetAsync(new ServerId(req.ServerId), ct);
        if (server is null) return false;

        server.SetStatus(req.Status);
        server.UpdateHealth(req.Reachable, req.PeersActive);

        return true;
    }
}