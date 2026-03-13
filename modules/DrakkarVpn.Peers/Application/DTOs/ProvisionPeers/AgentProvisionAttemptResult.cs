namespace DrakkarVpn.Core.Api.Modules.Peers.Application.DTOs.ProvisionPeers;

public sealed record AgentProvisionAttemptResult(
    Guid JobId,
    bool Applied,
    string? ErrorCode,
    string? ErrorMessage,
    DateTime AttemptedAtUtc
);