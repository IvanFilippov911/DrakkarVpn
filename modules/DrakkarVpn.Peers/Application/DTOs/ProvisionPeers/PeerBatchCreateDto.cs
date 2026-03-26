namespace DrakkarVpn.Core.Api.Modules.Peers.Application.DTOs.ProvisionPeers;

public record PeerBatchCreateDto(
    Guid JobId,
    Guid UserId,           
    Guid ServerId,
    Guid AgentPeerUuid,
    string DeviceId
);