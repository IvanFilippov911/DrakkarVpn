using DrakkarVpn.Core.Api.Modules.Servers.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Servers.Domain;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Servers.Application.Features.Commands.ChangeServerStatus;

public sealed class ChangeServerStatusHandler : IRequestHandler<ChangeServerStatusRequest, bool>
{
    private readonly IServerRepository _repo;
    public ChangeServerStatusHandler(IServerRepository repo) => _repo = repo;

    public async Task<bool> Handle(ChangeServerStatusRequest req, CancellationToken ct)
    {
        var server = await _repo.GetAsync(new ServerId(req.ServerId), ct);
        if (server is null) return false;
        server.SetStatus(req.Status);
        return true;
    }
}