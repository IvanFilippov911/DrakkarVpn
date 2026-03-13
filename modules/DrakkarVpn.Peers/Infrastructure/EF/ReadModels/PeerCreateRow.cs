namespace DrakkarVpn.Core.Api.Modules.Peers.Application.DTOs;

public sealed record PeerCreateRow(
    Guid UserId,
    Guid ServerId,
    Guid AgentPeerUuid,
    string ConfigRaw,
    string DeviceId,
    DateTime MarkerUtc
);