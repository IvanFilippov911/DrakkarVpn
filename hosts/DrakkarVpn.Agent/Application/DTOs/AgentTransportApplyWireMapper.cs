using DrakkarVpn.Agent.Application.DTOs.Enums;

namespace DrakkarVpn.Agent.Application.DTOs;

public static class AgentTransportApplyWireMapper
{
    public static AgentTransportApplyWireResponse ToWireResponse(AgentTransportApplyResult result)
        => new(
            Success: result.IsSuccess,
            Outcome: MapOutcome(result.Outcome),
            Code: result.Code,
            Message: result.Message,
            Phase: result.Phase.ToString(),
            PayloadHash: result.PayloadHash,
            RollbackAttempted: result.RollbackAttempted,
            RollbackSucceeded: result.RollbackSucceeded);

    private static string MapOutcome(AgentTransportApplyOutcome outcome)
        => outcome switch
        {
            AgentTransportApplyOutcome.Applied => "applied",
            AgentTransportApplyOutcome.AlreadyApplied => "already_applied",
            AgentTransportApplyOutcome.Rejected => "rejected",
            AgentTransportApplyOutcome.Failed => "failed",
            _ => "failed"
        };
}
