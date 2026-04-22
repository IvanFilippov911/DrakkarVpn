using DrakkarVpn.Servers.Application.Abstractions.Services;
using DrakkarVpn.Servers.Application.DTOs.ServerTransportActivations;
using MediatR;

namespace DrakkarVpn.Admin.Api.Application.Features.Commands.ServerTransportActivations.UpdateServerTransportActivation;

public sealed class UpdateServerTransportActivationHandler
    : IRequestHandler<UpdateServerTransportActivationRequest, Unit>
{
    private readonly IServerTransportActivationManagementService _service;

    public UpdateServerTransportActivationHandler(IServerTransportActivationManagementService service)
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
