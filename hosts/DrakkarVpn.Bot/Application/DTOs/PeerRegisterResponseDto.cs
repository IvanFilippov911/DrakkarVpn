namespace DrakkarVpn.Bot.Application.DTOs;

public sealed record PeerRegisterResponseDto(
    Guid Id,
    Guid ServerId,
    string ConfigRaw,
    DateTime CreatedAt,
    DateTime? ExpiresAt
);