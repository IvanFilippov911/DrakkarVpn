namespace DrakkarVpn.Core.Api.Modules.Peers.Application.DTOs;

public sealed record GetTgPeersDto(
    Guid Id,
    string ConfigRaw,
    DateTime CreatedAt
);