namespace DrakkarVpn.Core.Api.Modules.Peers.Application.DTOs;

public sealed record PeerProvisionJobCreateDto(
    Guid UserId,
    string DeviceId,
    Guid ServerId,
    int MaxAttempts,
    Guid AgentPeerUuid
);