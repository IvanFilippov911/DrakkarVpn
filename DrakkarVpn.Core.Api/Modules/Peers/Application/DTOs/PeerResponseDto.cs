using DrakkarVpn.Core.Api.Modules.Peers.Domain;

namespace DrakkarVpn.Core.Api.Modules.Peers.Application.DTOs;

public sealed record PeerResponseDto(
    Guid Id,
    Guid UserId,
    Guid ServerId,
    Guid AgentPeerId,
    PeerStatus Status,
    string ConfigRaw,
    DateTime CreatedAt,
    DateTime? ExpiresAt
);