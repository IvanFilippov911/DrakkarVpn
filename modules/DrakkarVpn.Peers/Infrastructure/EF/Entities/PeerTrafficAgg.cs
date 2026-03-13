using DrakkarVpn.Core.Api.Modules.Peers.Domain;

namespace DrakkarVpn.Core.Api.Modules.Peers.Infrastructure.Entities;

public sealed class PeerTrafficAgg
{
    public Guid PeerId { get; set; }

    public long Last1hBytes  { get; set; }
    public long Last24hBytes { get; set; }

    public DateTime UpdatedAtUtc { get; set; }
    
    public Peer Peer { get; set; } = null!;
}