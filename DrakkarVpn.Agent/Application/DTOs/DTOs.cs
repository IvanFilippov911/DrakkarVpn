namespace DrakkarVpn.Agent.Application.DTOs;

public sealed record RegisterPeerRequestDto(Guid UserId);

public sealed record RegisterPeerResponseDto(Guid PeerUuid, string ConfigRaw);

public sealed record RevokePeerResponseDto(bool Success);