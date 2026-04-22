using MediatR;
using NetworkMonitoring.Application.Abstractions.Services;
using NetworkMonitoring.Application.DTOs;

namespace DrakkarVpn.Admin.Api.Application.Features.Commands.NetworkMonitoring.UpdateProbeNode;

public sealed class UpdateProbeNodeHandler : IRequestHandler<UpdateProbeNodeRequest, bool>
{
    private readonly IProbeNodeManagementService _service;

    public UpdateProbeNodeHandler(IProbeNodeManagementService service)
        => _service = service;

    public Task<bool> Handle(UpdateProbeNodeRequest c, CancellationToken ct)
        => _service.UpdateAsync(c.ProbeNodeId, c.Data, ct);
}

