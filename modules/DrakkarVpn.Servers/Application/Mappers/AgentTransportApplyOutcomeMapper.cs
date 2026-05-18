using DrakkarVpn.Servers.Application.DTOs.ServerTransportApplyJobs;
using DrakkarVpn.Servers.Application.DTOs.TransportProfiles;
using DrakkarVpn.Servers.Application.DTOs.TransportProfiles.AgentApply;

public static class AgentTransportApplyOutcomeMapper
{
    public static AgentApplyResult ToJobResult(AgentTransportApplyCallResult call)
    {
        ArgumentNullException.ThrowIfNull(call);

        if (call.TransportErrorCode is not null)
        {
            return Retryable(
                call.TransportErrorCode,
                call.TransportErrorMessage);
        }

        if (call.Wire is null)
        {
            return Retryable(
                "agent_response_missing",
                "Agent apply response is missing.");
        }

        return MapWire(call.Wire);
    }

    private static AgentApplyResult MapWire(AgentApplyServerTransportWireResponse wire)
    {
        if (wire.IsAppliedOutcome)
            return AppliedFromWire(wire);

        if (wire.IsRejectedOutcome)
            return PermanentFromWire(wire);

        if (wire.IsFailedOutcome)
            return RetryableFromWire(wire);

        if (!wire.Success)
        {
            return RetryableFromWire(
                wire,
                fallbackCode: wire.Code ?? "agent_apply_failed",
                fallbackMessage: wire.Message ?? "Agent apply did not succeed.");
        }

        return RetryableFromWire(
            wire,
            fallbackCode: wire.Code ?? "agent_unknown_outcome",
            fallbackMessage: wire.Message ?? $"Unknown agent outcome '{wire.Outcome}'.");
    }

    private static AgentApplyResult AppliedFromWire(AgentApplyServerTransportWireResponse wire)
        => new(
            AgentApplyOutcome.Applied,
            PayloadHash: wire.PayloadHash,
            Phase: wire.Phase,
            RollbackAttempted: wire.RollbackAttempted,
            RollbackSucceeded: wire.RollbackSucceeded);

    private static AgentApplyResult PermanentFromWire(AgentApplyServerTransportWireResponse wire)
        => new(
            AgentApplyOutcome.PermanentFailed,
            ErrorCode: wire.Code ?? "agent_rejected",
            ErrorMessage: wire.Message,
            Phase: wire.Phase,
            PayloadHash: wire.PayloadHash,
            RollbackAttempted: wire.RollbackAttempted,
            RollbackSucceeded: wire.RollbackSucceeded);

    private static AgentApplyResult RetryableFromWire(
        AgentApplyServerTransportWireResponse wire,
        string? fallbackCode = null,
        string? fallbackMessage = null)
        => new(
            AgentApplyOutcome.RetryableFailed,
            ErrorCode: wire.Code ?? fallbackCode ?? "agent_failed",
            ErrorMessage: wire.Message ?? fallbackMessage,
            Phase: wire.Phase,
            PayloadHash: wire.PayloadHash,
            RollbackAttempted: wire.RollbackAttempted,
            RollbackSucceeded: wire.RollbackSucceeded);

    private static AgentApplyResult Retryable(string code, string? message)
        => new(
            AgentApplyOutcome.RetryableFailed,
            ErrorCode: code,
            ErrorMessage: message);
}