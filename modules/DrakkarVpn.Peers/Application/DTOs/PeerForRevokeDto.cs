namespace DrakkarVpn.Core.Api.Modules.Peers.Application.DTOs;

public sealed record PeerForRevokeDto(
    Guid Id,
    Guid ServerId,
    int Status
);