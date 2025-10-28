namespace DrakkarVpn.Shared.Peers;

public sealed record PeerBriefDto(
    Guid Id,
    Guid AgentPeerId,
    short Status,
    DateTime? LastHandshakeAt
);