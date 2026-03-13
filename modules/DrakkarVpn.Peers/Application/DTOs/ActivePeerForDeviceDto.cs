namespace DrakkarVpn.Core.Api.Modules.Peers.Application.DTOs;

public sealed record ActivePeerForDeviceDto(
    Guid PeerId,
    Guid ServerId
);