namespace DrakkarVpn.Core.Api.Modules.Peers.Application.DTOs;

public sealed record PeerRegisterResponseDto(
    Guid AgentPeerId,   
    string ConfigRaw
);