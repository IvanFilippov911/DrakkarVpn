namespace DrakkarVpn.Core.Api.Modules.Peers.Infrastructure.Entities;

public sealed class PeerEntity
{
    public Guid Id { get; set; }
    public Guid ServerId { get; set; }
    public Guid AgentPeerUuid { get; set; }
    public string DeviceId { get; set; } = default!;
    public int Status { get; set; }

    public bool IsOnline { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime StatusUpdatedAtUtc { get; set; }
}