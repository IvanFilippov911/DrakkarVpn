namespace DrakkarVpn.Core.Api.Modules.Peers.Application.DTOs;

public sealed record PeerProvisionPayloadDto(Guid AgentPeerUuid, string ConfigRaw);