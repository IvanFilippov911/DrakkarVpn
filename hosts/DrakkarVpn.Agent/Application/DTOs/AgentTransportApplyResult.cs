using DrakkarVpn.Agent.Application.DTOs.Enums;

namespace DrakkarVpn.Agent.Application.DTOs;

public sealed record AgentTransportApplyResult
{
    public bool IsSuccess { get; init; }
    public AgentTransportApplyOutcome? Outcome { get; init; }
    public AgentTransportApplyErrorPhase Phase { get; init; } = AgentTransportApplyErrorPhase.None;

    public string? PayloadHash { get; init; }

    public string? ErrorCode { get; init; }
    public string? ErrorMessage { get; init; }

    public bool RollbackAttempted { get; init; }
    public bool RollbackSucceeded { get; init; }

    public bool IsClientError =>
        !IsSuccess && Phase is AgentTransportApplyErrorPhase.FluentValidation or AgentTransportApplyErrorPhase.Preconditions;

    public static AgentTransportApplyResult Ok(
        AgentTransportApplyOutcome outcome,
        string payloadHash,
        bool rollbackAttempted = false,
        bool rollbackSucceeded = false) =>
        new()
        {
            IsSuccess = true,
            Phase = AgentTransportApplyErrorPhase.None,
            Outcome = outcome,
            PayloadHash = payloadHash,
            RollbackAttempted = rollbackAttempted,
            RollbackSucceeded = rollbackSucceeded
        };

    public static AgentTransportApplyResult ClientError(
        AgentTransportApplyErrorPhase phase,
        string code,
        string message,
        string? payloadHash = null,
        bool rollbackAttempted = false,
        bool rollbackSucceeded = false) =>
        new()
        {
            IsSuccess = false,
            Phase = phase,
            ErrorCode = code,
            ErrorMessage = message,
            PayloadHash = payloadHash,
            RollbackAttempted = rollbackAttempted,
            RollbackSucceeded = rollbackSucceeded
        };

    public static AgentTransportApplyResult ServerFailure(
        AgentTransportApplyErrorPhase phase,
        string code,
        string message,
        string? payloadHash,
        bool rollbackAttempted = false,
        bool rollbackSucceeded = false) =>
        new()
        {
            IsSuccess = false,
            Phase = phase,
            ErrorCode = code,
            ErrorMessage = message,
            PayloadHash = payloadHash,
            RollbackAttempted = rollbackAttempted,
            RollbackSucceeded = rollbackSucceeded
        };
}
