namespace DrakkarVpn.Agent.Application.DTOs;

public sealed record AgentTransportStateDto(
    Guid ServerId,
    Guid ActivationId,
    Guid OperationId,
    string PayloadHash,
    DateTime AppliedAtUtc,
    DateTime UpdatedAtUtc);
