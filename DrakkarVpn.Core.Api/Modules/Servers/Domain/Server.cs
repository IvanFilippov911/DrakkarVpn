using DrakkarVpn.Core.Api.Modules.Servers.Domain.Abstractions;

namespace DrakkarVpn.Core.Api.Modules.Servers.Domain;

public sealed class Server : IAggregateRoot
{
    private Server() { }
    private Server(
        ServerId id,
        string name,
        Region region,
        PublicHost host,
        Uri agentBaseUrl,
        string agentTokenEncrypted,
        int? maxPeers)
    {
        Id = id;
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Region = region;
        PublicHost = host;
        AgentBaseUrl = agentBaseUrl ?? throw new ArgumentNullException(nameof(agentBaseUrl));
        AgentTokenEncrypted = string.IsNullOrWhiteSpace(agentTokenEncrypted)
            ? throw new ArgumentNullException(nameof(agentTokenEncrypted))
            : agentTokenEncrypted;

        MaxPeers = maxPeers;
        Status = ServerStatus.Enabled;
        Health = new HealthSnapshot(true, 0, DateTime.MinValue);
        CreatedAt = DateTime.UtcNow;
    }
    
    public ServerId Id { get; }
    public string Name { get; private set; }
    public Region Region { get; private set; }
    public PublicHost PublicHost { get; private set; }
    public Uri AgentBaseUrl { get; private set; }
    public string AgentTokenEncrypted { get; private set; }
    public ServerStatus Status { get; private set; }
    public int? MaxPeers { get; private set; }
    public HealthSnapshot Health { get; private set; } = default!;

    public DateTime CreatedAt { get; }

    
    public static Server Register(
        ServerId id,
        string name,
        Region region,
        PublicHost host,
        Uri agentBaseUrl,
        string agentTokenEncrypted,
        int? maxPeers)
        => new(id, name, region, host, agentBaseUrl, agentTokenEncrypted, maxPeers);

    
    public void SetStatus(ServerStatus status) => Status = status;

    public void SetCapacity(int? maxPeers)
    {
        if (maxPeers is < 0) throw new ArgumentOutOfRangeException(nameof(maxPeers));
        MaxPeers = maxPeers;
    }

    public void UpdateHealth(bool reachable, int peersActive)
    {
        if (peersActive < 0) peersActive = 0;
        Health = new HealthSnapshot(reachable, peersActive, DateTime.UtcNow);
    }
    
    public bool IsEligible() =>
        Status == ServerStatus.Enabled
        && Health.Reachable
        && (MaxPeers is null || Health.PeersActive < MaxPeers);
}