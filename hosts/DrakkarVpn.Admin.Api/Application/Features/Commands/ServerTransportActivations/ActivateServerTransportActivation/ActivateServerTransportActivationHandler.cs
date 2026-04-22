using DrakkarVpn.Servers.Application.Abstractions.Services;
using DrakkarVpn.Servers.Application.DTOs.ServerTransportActivations;
using MediatR;

namespace DrakkarVpn.Admin.Api.Application.Features.Commands.ServerTransportActivations.ActivateServerTransportActivation;

public sealed class ActivateServerTransportActivationHandler
    : IRequestHandler<ActivateServerTransportActivationRequest, Unit>
{
    private readonly IServerTransportActivationManagementService _service;

    public ActivateServerTransportActivationHandler(IServerTransportActivationManagementService service)
        => _service = service;

    public async Task<Unit> Handle(ActivateServerTransportActivationRequest command, CancellationToken ct)
    {
        await _service.ActivateAsync(
            new ActivateServerTransportActivationInput(
                ServerId: command.ServerId,
                ActivationId: command.ActivationId),
            ct);
        
        

        return Unit.Value;
    }
}
