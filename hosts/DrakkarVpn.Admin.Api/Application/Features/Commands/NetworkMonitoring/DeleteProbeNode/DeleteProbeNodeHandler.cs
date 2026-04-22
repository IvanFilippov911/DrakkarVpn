using MediatR;
using NetworkMonitoring.Application.Abstractions.Services;

namespace DrakkarVpn.Admin.Api.Application.Features.Commands.NetworkMonitoring.DeleteProbeNode;

public sealed class DeleteProbeNodeHandler : IRequestHandler<DeleteProbeNodeRequest, bool>
{
    private readonly IProbeNodeManagementService _service;

    public DeleteProbeNodeHandler(IProbeNodeManagementService service)
        => _service = service;

    public Task<bool> Handle(DeleteProbeNodeRequest c, CancellationToken ct)
        => _service.DeleteAsync(c.ProbeNodeId, ct);
}

