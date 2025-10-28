using DrakkarVpn.Core.Api.Modules.Servers.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Servers.Domain;
using DrakkarVpn.Core.Api.Modules.Servers.Domain.VO;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Servers.Application.Features.Commands.RegisterServer;

public sealed class RegisterServerHandler : IRequestHandler<RegisterServerRequest, Guid>
{
    private readonly IServerRepository _repo;
    public RegisterServerHandler(IServerRepository repo) => _repo = repo;

    public async Task<Guid> Handle(RegisterServerRequest req, CancellationToken ct)
    {
        var entity = Server.Register(
            Guid.NewGuid(),
            req.Name,
            new Region(req.Region),
            new PublicHost(req.PublicHost),
            new Uri(req.AgentBaseUrl),
            req.AgentTokenEncrypted,
            req.MaxPeers);

        await _repo.AddAsync(entity, ct);
        return entity.Id;
    }
}