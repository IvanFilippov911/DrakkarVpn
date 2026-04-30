using DrakkarVpn.Servers.Application.Abstractions.Services;
using DrakkarVpn.Servers.Application.DTOs.ServerTransportActivations;
using MediatR;

namespace DrakkarVpn.Admin.Api.Application.Features.Commands.ServerTransportActivations.ActivateServerTransportActivation;

public sealed class ActivateServerTransportHandler
    : IRequestHandler<ActivateServerTransportRequest, Unit>
{
    private readonly IServerTransportActivationManagementService _transportService;
    private readonly IServerTransportApplyJobService _jobService;

    public ActivateServerTransportHandler(
        IServerTransportActivationManagementService transportService,
        IServerTransportApplyJobService jobService)
    { 
        _transportService = transportService;
        _jobService = jobService;
    }

    public async Task<Unit> Handle(ActivateServerTransportRequest command, CancellationToken ct)
    {
        await _transportService.ActivateAsync(
            new ActivateServerTransportActivationInput(
                ServerId: command.ServerId,
                ActivationId: command.ActivationId),
            ct);

        await _jobService.EnqueueAsync(command.ServerId, command.ActivationId, ct);

        return Unit.Value;
    }
}
