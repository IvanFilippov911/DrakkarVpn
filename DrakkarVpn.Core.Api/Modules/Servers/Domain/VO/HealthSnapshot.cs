using Microsoft.EntityFrameworkCore;

namespace DrakkarVpn.Core.Api.Modules.Servers.Domain.VO;

[Owned]
public sealed class HealthSnapshot
{
    public bool Reachable { get; private set; }
    public int PeersActive { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private HealthSnapshot() {} 
    
    public HealthSnapshot(bool reachable, int peersActive, DateTime updatedAt)
    {
        Reachable = reachable;
        PeersActive = peersActive;
        UpdatedAt = updatedAt;
    }
}