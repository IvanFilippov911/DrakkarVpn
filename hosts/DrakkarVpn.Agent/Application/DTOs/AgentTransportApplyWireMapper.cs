using DrakkarVpn.Agent.Application.DTOs.Enums;

namespace DrakkarVpn.Agent.Application.DTOs;

public static class AgentTransportApplyWireMapper
{
    public static AgentTransportApplyWireResponse ToWireResponse(AgentTransportApplyResult result)
        => new(
            Success: result.IsSuccess,
            Outcome: result.Outcome switch
            {
                AgentTransportApplyOutcome.Applied => "applied",
                AgentTransportApplyOutcome.AlreadyApplied => "already_applied",
                _ => null
            },
            Code: result.ErrorCode,
            Message: result.ErrorMessage,
            Phase: result.Phase.ToString(),
            PayloadHash: result.PayloadHash,
            RollbackAttempted: result.RollbackAttempted,
            RollbackSucceeded: result.RollbackSucceeded);
}
