using DrakkarVpn.Core.Api.Modules.Peers.Domain.enums;

namespace DrakkarVpn.Core.Api.Modules.Peers.Application.DTOs;

public sealed record PeerSyncIssueItem(
    Guid ServerId,
    Guid? PeerId,
    Guid? AgentPeerId,
    PeerSyncIssueType Type,
    DateTime DetectedAtUtc,
    string? Details
);