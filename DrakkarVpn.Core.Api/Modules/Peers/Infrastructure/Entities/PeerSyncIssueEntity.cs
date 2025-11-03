using DrakkarVpn.Core.Api.Modules.Peers.Domain.enums;

namespace DrakkarVpn.Core.Api.Modules.Peers.Infrastructure.Entities;

public class PeerSyncIssueEntity
{
    public Guid Id { get; set; }
    public Guid ServerId { get; set; }
    public Guid? PeerId { get; set; }
    public Guid? AgentPeerUuid { get; set; }
    public PeerSyncIssueType Type { get; set; }
    public DateTime DetectedAtUtc { get; set; }
    public string? Details { get; set; }
}