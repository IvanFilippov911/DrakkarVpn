namespace DrakkarVpn.Core.Api.Modules.Peers.Application.DTOs;

public sealed record AdminUserPeerShortDto(
    Guid   PeerId,
    Guid   ServerId,
    Guid   AgentPeerUuid,
    bool   IsOnline,
    long   Traffic24hBytes
);