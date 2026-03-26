using DrakkarVpn.Shared.Peers;

namespace DrakkarVpn.Core.Api.Modules.Peers.API.Contracts.Response;

public sealed record PeerProvisionJobResponse(
    PeerProvisionStatus Status,
    string? ErrorCode,
    string? ErrorMessage
);