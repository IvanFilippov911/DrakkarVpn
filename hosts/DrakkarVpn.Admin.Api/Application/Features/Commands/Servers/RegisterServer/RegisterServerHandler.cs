using DrakkarVpn.Core.Api.Modules.Servers.Application.Abstractions;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Commands.Servers.RegisterServer;

public sealed class RegisterServerHandler
    : IRequestHandler<RegisterServerRequest, Guid>
{
    private readonly IServerManagementService _service;

    public RegisterServerHandler(IServerManagementService service)
        => _service = service;

    public Task<Guid> Handle(RegisterServerRequest c, CancellationToken ct)
        => _service.RegisterAsync(
            c.Name,
            c.Region,
            c.PublicHost,
            c.AgentBaseUrl,
            c.AgentTokenEncrypted,
            c.MaxPeers,
            ct);
}