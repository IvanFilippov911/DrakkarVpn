namespace DrakkarVpn.Servers.Application.DTOs.ServerTransportApplyJobs;

public sealed record AgentApplyResult(
    AgentApplyOutcome Outcome,
    string? ErrorCode = null,
    string? ErrorMessage = null,
    string? Phase = null,
    string? PayloadHash = null,
    bool? RollbackAttempted = null,
    bool? RollbackSucceeded = null);