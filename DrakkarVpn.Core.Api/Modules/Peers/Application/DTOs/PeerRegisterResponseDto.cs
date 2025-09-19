namespace DrakkarVpn.Core.Api.Modules.Peers.Application.DTOs;

public sealed record PeerRegisterResponseDto(
    Guid Id,
    Guid ServerId,
    string ConfigRaw,
    DateTime CreatedAt,
    DateTime? ExpiresAt
);