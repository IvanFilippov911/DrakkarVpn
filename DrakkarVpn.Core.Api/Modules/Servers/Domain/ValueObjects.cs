using Microsoft.EntityFrameworkCore;

namespace DrakkarVpn.Core.Api.Modules.Servers.Domain;

public readonly record struct ServerId(Guid Value)
{
    public static ServerId New() => new(Guid.NewGuid());
    public override string ToString() => Value.ToString();
}

public readonly record struct Region(string Code)
{
    public override string ToString() => Code;
}

public readonly record struct PublicHost(string Value)
{
    public override string ToString() => Value;
}

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