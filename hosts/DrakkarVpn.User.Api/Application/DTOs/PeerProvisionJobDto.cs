using DrakkarVpn.Shared.Peers;

namespace DrakkarVpn.Core.Api.Modules.Orchestrator.Application.DTOs;

public sealed record PeerProvisionJobDto(
    Guid JobId,
    PeerProvisionStatus Status,
    int Attempt,
    int MaxAttempts,
    DateTime NextAttemptAtUtc,
    Guid? PeerId,
    Guid? AgentPeerUuid,
    string? ConfigRaw,
    string? HappLink,
    string? ErrorCode,
    string? ErrorMessage);
