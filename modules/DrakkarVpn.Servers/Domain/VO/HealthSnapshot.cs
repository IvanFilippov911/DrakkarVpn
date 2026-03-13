using Microsoft.EntityFrameworkCore;

namespace DrakkarVpn.Core.Api.Modules.Servers.Domain.VO;

[Owned]
public sealed class HealthSnapshot
{
    public bool Reachable { get; private set; }
    public int PeersActive { get; private set; }
    public DateTime LastHealthAtUtc { get; private set; }

    private HealthSnapshot() {} 
    
    public HealthSnapshot(bool reachable, int peersActive, DateTime LastHealth)
    {
        Reachable = reachable;
        PeersActive = peersActive;
        LastHealthAtUtc = LastHealth;
    }
}