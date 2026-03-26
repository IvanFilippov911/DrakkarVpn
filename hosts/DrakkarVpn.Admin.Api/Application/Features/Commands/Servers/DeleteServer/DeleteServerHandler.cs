using DrakkarVpn.Core.Api.Modules.Servers.Application.Abstractions;
using DrakkarVpn.Servers.Application.Abstractions.Services;
using DrakkarVpn.Shared.Servers;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Commands.Servers.DeleteServer;

public sealed class DeleteServerHandler
    : IRequestHandler<DeleteServerRequest, bool>
{
    private readonly IServerManagementService _service;

    public DeleteServerHandler(IServerManagementService service)
        => _service = service;

    public async Task<bool> Handle(DeleteServerRequest c, CancellationToken ct)
    {
        var result = await _service.DeleteAsync(c.ServerId, ct);
        return result == DeleteServerResult.Deleted;
    }
}