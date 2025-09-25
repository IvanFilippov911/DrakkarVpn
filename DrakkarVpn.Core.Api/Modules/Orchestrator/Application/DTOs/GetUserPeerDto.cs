namespace DrakkarVpn.Core.Api.Modules.Orchestrator.Application.DTOs;

public sealed record GetUserPeerDto(
    Guid Id,
    Guid ServerId,
    string Status,
    string ConfigRaw,
    DateTime CreatedAt,
    DateTime? ExpiresAt
);