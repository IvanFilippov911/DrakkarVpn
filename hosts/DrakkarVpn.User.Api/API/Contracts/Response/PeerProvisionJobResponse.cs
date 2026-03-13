using DrakkarVpn.Shared.Peers;

namespace DrakkarVpn.Core.Api.Modules.Peers.API.Contracts.Response;

public sealed record PeerProvisionJobResponse(
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
    string? ErrorMessage
);