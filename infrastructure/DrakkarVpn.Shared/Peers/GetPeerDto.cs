namespace DrakkarVpn.Shared.Peers;

public sealed record GetPeerDto(
    Guid Id,
    Guid ServerId,
    string DeviceId,
    Guid AgentPeerId,
    string ConfigRaw,
    int Status,                 
    DateTime CreatedAt
);