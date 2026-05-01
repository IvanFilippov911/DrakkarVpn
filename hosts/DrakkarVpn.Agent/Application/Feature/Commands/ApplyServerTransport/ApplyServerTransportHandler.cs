using DrakkarVpn.Agent.Application.Abstractions.AgentTransport;
using DrakkarVpn.Agent.Application.DTOs;
using DrakkarVpn.Agent.Application.DTOs.Enums;
using FluentValidation;
using MediatR;

namespace DrakkarVpn.Agent.Application.Feature.Commands.ApplyServerTransport;

public sealed class ApplyServerTransportHandler
    : IRequestHandler<ApplyServerTransportCommand, AgentTransportApplyResult>
{
    private readonly IAgentTransportApplyService _apply;
    private readonly IValidator<ApplyServerTransportCommand> _validator;

    public ApplyServerTransportHandler(
        IAgentTransportApplyService apply,
        IValidator<ApplyServerTransportCommand> validator)
    {
        _apply = apply;
        _validator = validator;
    }

    public async Task<AgentTransportApplyResult> Handle(ApplyServerTransportCommand request, CancellationToken ct)
    {
        var validation = await _validator.ValidateAsync(request, ct);
        if (!validation.IsValid)
        {
            var first = validation.Errors[0];
            return AgentTransportApplyResult.ClientError(
                AgentTransportApplyErrorPhase.FluentValidation,
                string.IsNullOrWhiteSpace(first.ErrorCode) ? "validation_error" : first.ErrorCode,
                first.ErrorMessage);
        }

        return await _apply.ApplyAsync(request.Body, ct);
    }
}
