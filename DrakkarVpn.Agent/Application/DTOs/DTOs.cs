namespace DrakkarVpn.Agent.Application.DTOs;

public sealed record RegisterPeerResponseDto(Guid PeerUuid, string ConfigRaw);

public sealed record RevokePeerResponseDto(bool Success);

public sealed record PeersResultDto(Guid Uuid, string Email, string Flow, string Encryption);