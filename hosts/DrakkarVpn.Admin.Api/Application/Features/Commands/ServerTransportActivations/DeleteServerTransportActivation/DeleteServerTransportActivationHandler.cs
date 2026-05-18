using DrakkarVpn.Servers.Application.Abstractions.Services.ServerTransportActivations;
using DrakkarVpn.Servers.Application.DTOs.ServerTransportActivations;
using MediatR;

namespace DrakkarVpn.Admin.Api.Application.Features.Commands.ServerTransportActivations.DeleteServerTransportActivation;

public sealed class DeleteServerTransportActivationHandler
    : IRequestHandler<DeleteServerTransportActivationRequest, Unit>
{
    private readonly IServerTransportManagementService _service;

    public DeleteServerTransportActivationHandler(IServerTransportManagementService service)
        => _service = service;

    public async Task<Unit> Handle(DeleteServerTransportActivationRequest command, CancellationToken ct)
    {
        await _service.DetachAsync(
            new DetachServerTransportActivationInput(
                ServerId: command.ServerId,
                ActivationId: command.ActivationId),
            ct);

        return Unit.Value;
    }
}
