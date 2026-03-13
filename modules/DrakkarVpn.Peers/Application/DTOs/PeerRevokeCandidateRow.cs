namespace DrakkarVpn.Core.Api.Modules.Peers.Application.DTOs;

public sealed record PeerRevokeCandidateRow(
    Guid UserId,
    string DeviceId,
    Guid PeerId,
    Guid ServerId,
    Guid AgentPeerUuid
);