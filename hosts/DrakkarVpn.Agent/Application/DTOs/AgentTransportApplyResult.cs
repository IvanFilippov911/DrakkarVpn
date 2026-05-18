using DrakkarVpn.Agent.Application.DTOs.Enums;

namespace DrakkarVpn.Agent.Application.DTOs;

public sealed record AgentTransportApplyResult
{
    public AgentTransportApplyOutcome Outcome { get; init; }
    public AgentTransportApplyPhase Phase { get; init; } = AgentTransportApplyPhase.None;

    public string? Code { get; init; }
    public string? Message { get; init; }

    public string? PayloadHash { get; init; }

    public bool RollbackAttempted { get; init; }
    public bool RollbackSucceeded { get; init; }

    public bool IsSuccess =>
        Outcome is AgentTransportApplyOutcome.Applied
            or AgentTransportApplyOutcome.AlreadyApplied;

    public bool IsRejected => Outcome == AgentTransportApplyOutcome.Rejected;

    public static AgentTransportApplyResult Applied(string payloadHash) => new()
    {
        Outcome = AgentTransportApplyOutcome.Applied,
        PayloadHash = payloadHash
    };

    public static AgentTransportApplyResult AlreadyApplied(string payloadHash) => new()
    {
        Outcome = AgentTransportApplyOutcome.AlreadyApplied,
        PayloadHash = payloadHash
    };

    public static AgentTransportApplyResult Rejected(
        AgentTransportApplyPhase phase,
        string code,
        string message,
        string? payloadHash = null) => new()
    {
        Outcome = AgentTransportApplyOutcome.Rejected,
        Phase = phase,
        Code = code,
        Message = message,
        PayloadHash = payloadHash
    };

    public static AgentTransportApplyResult Failed(
        AgentTransportApplyPhase phase,
        string code,
        string message,
        string? payloadHash = null,
        bool rollbackAttempted = false,
        bool rollbackSucceeded = false) => new()
    {
        Outcome = AgentTransportApplyOutcome.Failed,
        Phase = phase,
        Code = code,
        Message = message,
        PayloadHash = payloadHash,
        RollbackAttempted = rollbackAttempted,
        RollbackSucceeded = rollbackSucceeded
    };
}
