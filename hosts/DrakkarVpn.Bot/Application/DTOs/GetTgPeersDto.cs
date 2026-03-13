namespace DrakkarVpn.Bot.Application.DTOs;

public sealed record GetTgPeersDto(
    Guid Id,
    string ConfigRaw,
    DateTime CreatedAt,
    DateTime? ExpiresAt
);