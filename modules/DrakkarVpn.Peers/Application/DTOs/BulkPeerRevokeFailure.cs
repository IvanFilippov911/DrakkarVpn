namespace DrakkarVpn.Core.Api.Modules.Peers.Application.DTOs;

public sealed record BulkPeerRevokeFailure(
    Guid UserId,
    Guid PeerId,
    Guid ServerId,
    string Reason,
    string? Message = null
);