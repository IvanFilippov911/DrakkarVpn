using DrakkarVpn.Servers.Application.Abstractions.Services.ServerTransportActivations;
using DrakkarVpn.Servers.Application.DTOs.ServerTransportActivations;
using MediatR;

namespace DrakkarVpn.Admin.Api.Application.Features.Commands.ServerTransportActivations.UpdateServerTransportActivation;

public sealed class UpdateServerTransportActivationHandler
    : IRequestHandler<UpdateServerTransportActivationRequest, Unit>
{
    private readonly IServerTransportManagementService _service;

    public UpdateServerTransportActivationHandler(IServerTransportManagementService service)
        => _service = service;

    public async Task<Unit> Handle(UpdateServerTransportActivationRequest command, CancellationToken ct)
    {
        await _service.UpdateAsync(
            new UpdateServerTransportActivationInput(
                ServerId: command.ServerId,
                ActivationId: command.ActivationId,
                RealityPublicKey: command.RealityPublicKey,
                LocalPriority: command.LocalPriority),
            ct);

        return Unit.Value;
    }
}
