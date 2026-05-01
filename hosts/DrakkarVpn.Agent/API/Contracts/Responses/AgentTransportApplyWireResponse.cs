namespace DrakkarVpn.Agent.Application.DTOs;

public sealed record AgentTransportApplyWireResponse(
    bool Success,
    string? Outcome,
    string? Code,
    string? Message,
    string Phase,
    string? PayloadHash,
    bool RollbackAttempted,
    bool RollbackSucceeded);
