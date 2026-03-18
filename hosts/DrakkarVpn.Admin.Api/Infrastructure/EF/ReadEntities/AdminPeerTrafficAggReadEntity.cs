namespace DrakkarVpn.Admin.Api.Infrastructure.EF.ReadEntities;

public sealed class AdminPeerTrafficAggReadEntity
{
    public Guid PeerId { get; set; }

    public long Last24hBytes { get; set; }
}

