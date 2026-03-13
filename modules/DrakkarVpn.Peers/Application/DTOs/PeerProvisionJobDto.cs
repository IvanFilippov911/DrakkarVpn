using DrakkarVpn.Core.Api.Modules.Peers.Domain.enums;
using DrakkarVpn.Shared.Peers;

namespace DrakkarVpn.Core.Api.Modules.Peers.Application.DTOs;

public sealed record PeerProvisionJobDto(
    Guid JobId,
    PeerProvisionState State,
    int Attempt,
    int MaxAttempts,
    DateTime NextAttemptAtUtc,
    Guid? PeerId,
    Guid? AgentPeerUuid,
    string? ConfigRaw,
    string? ErrorCode,
    string? ErrorMessage
);