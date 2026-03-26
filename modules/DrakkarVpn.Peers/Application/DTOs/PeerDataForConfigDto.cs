namespace DrakkarVpn.Core.Api.Modules.Peers.Application.DTOs;

public sealed record PeerDataForConfigDto(
    Guid ServerId,
    Guid AgentUuid);

