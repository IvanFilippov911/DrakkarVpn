namespace DrakkarVpn.Core.Api.Modules.Peers.Application.DTOs.ProvisionPeers;

public record PeerCreationResult(
    Guid JobId,
    bool Success,
    Guid? PeerId = null,
    string? ErrorCode = null,
    string? ErrorMessage = null
);