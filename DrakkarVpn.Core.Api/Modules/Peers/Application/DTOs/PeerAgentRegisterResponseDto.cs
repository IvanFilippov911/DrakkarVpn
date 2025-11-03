using DrakkarVpn.Core.Api.Modules.Peers.Domain;

namespace DrakkarVpn.Core.Api.Modules.Peers.Application.DTOs;

public sealed record PeerAgentRegisterResponseDto(
    Guid PeerUuid,
    string ConfigRaw
);