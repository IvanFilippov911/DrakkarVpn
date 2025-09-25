namespace DrakkarVpn.Agent.Application.DTOs;

public sealed record RegisterPeerResponseDto(Guid PeerUuid, string ConfigRaw);