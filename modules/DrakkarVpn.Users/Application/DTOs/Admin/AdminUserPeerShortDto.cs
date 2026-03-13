namespace DrakkarVpn.Users.Application.DTOs.Admin;

public sealed record AdminUserPeerShortDto(
    Guid   PeerId,
    Guid   ServerId,
    Guid   AgentPeerUuid,
    bool   IsOnline,
    long   Traffic24hBytes
);