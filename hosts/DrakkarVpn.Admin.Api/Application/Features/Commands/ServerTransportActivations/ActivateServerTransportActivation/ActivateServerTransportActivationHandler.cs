using DrakkarVpn.Servers.Application.Abstractions.Services;
using DrakkarVpn.Servers.Application.DTOs.ServerTransportActivations;
using MediatR;

namespace DrakkarVpn.Admin.Api.Application.Features.Commands.ServerTransportActivations.ActivateServerTransportActivation;

public sealed class ActivateServerTransportActivationHandler
    : IRequestHandler<ActivateServerTransportActivationRequest, Unit>
{
    private readonly IServerTransportActivationManagementService _transportService;
    private readonly IAgentApplyServerTransportService _agentApplyService;

    public ActivateServerTransportActivationHandler(
        IServerTransportActivationManagementService transportService,
        IAgentApplyServerTransportService agentApplyService)
    { 
        _transportService = transportService;
        _agentApplyService = agentApplyService;
    }

    public async Task<Unit> Handle(ActivateServerTransportActivationRequest command, CancellationToken ct)
    {
        await _transportService.ActivateAsync(
            new ActivateServerTransportActivationInput(
                ServerId: command.ServerId,
                ActivationId: command.ActivationId),
            ct);
        
        await _agentApplyService.ApplyAsync(command.ServerId, ct);
        

        return Unit.Value;
    }
}
