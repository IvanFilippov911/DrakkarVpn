using MediatR;
using NetworkMonitoring.Application.Abstractions.Services;

namespace DrakkarVpn.Admin.Api.Application.Features.Commands.NetworkMonitoring.RegisterProbeNode;

public sealed class RegisterProbeNodeHandler
    : IRequestHandler<RegisterProbeNodeRequest, Guid>
{
    private readonly IProbeNodeRegistrationService _service;

    public RegisterProbeNodeHandler(IProbeNodeRegistrationService service)
        => _service = service;

    public Task<Guid> Handle(RegisterProbeNodeRequest c, CancellationToken ct)
        => _service.RegisterAsync(c.Data, ct);
}

