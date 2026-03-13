namespace DrakkarVpn.Core.Api.Modules.Peers.Application.DTOs;

public sealed record BulkPeersRevokeResultDto(
    IReadOnlyList<Guid> SucceededUserIds,
    IReadOnlyList<Guid> FailedUserIds,
    IReadOnlyList<BulkPeerRevokeFailure> FailureDetails
);