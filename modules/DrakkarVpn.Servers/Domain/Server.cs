using DrakkarVpn.Core.Api.Modules.Servers.Domain.Abstractions;
using DrakkarVpn.Core.Api.Modules.Servers.Domain.VO;

namespace DrakkarVpn.Core.Api.Modules.Servers.Domain;

public sealed class Server : IAggregateRoot
{
    private Server() { }

    private Server(
        Guid id,
        string name,
        Region region,
        PublicHost host,
        int publicPort,
        string realityPublicKey,
        string realityShortId,
        string realitySni,
        Uri agentBaseUrl,
        string agentTokenEncrypted,
        int? maxPeers)
    {
        Id = id;
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Region = region;
        PublicHost = host;

        if (publicPort <= 0 || publicPort > 65535)
            throw new ArgumentOutOfRangeException(nameof(publicPort), "Public port must be in range 1..65535.");

        PublicPort = publicPort;
        RealityPublicKey = string.IsNullOrWhiteSpace(realityPublicKey)
            ? throw new ArgumentNullException(nameof(realityPublicKey))
            : realityPublicKey.Trim();

        RealityShortId = string.IsNullOrWhiteSpace(realityShortId)
            ? throw new ArgumentNullException(nameof(realityShortId))
            : realityShortId.Trim();

        RealitySni = string.IsNullOrWhiteSpace(realitySni)
            ? throw new ArgumentNullException(nameof(realitySni))
            : realitySni.Trim();

        AgentBaseUrl = agentBaseUrl ?? throw new ArgumentNullException(nameof(agentBaseUrl));
        AgentTokenEncrypted = string.IsNullOrWhiteSpace(agentTokenEncrypted)
            ? throw new ArgumentNullException(nameof(agentTokenEncrypted))
            : agentTokenEncrypted;

        MaxPeers = maxPeers;
        Status = ServerStatus.Disabled;
        Health = new HealthSnapshot(false, 0, DateTime.UtcNow);
        Metrics = MetricsSnapshot.Default;
        Benchmark = BenchmarkSnapshot.Empty;
        CreatedAt = DateTime.UtcNow;
    }

    public Guid Id { get; }
    public string Name { get; private set; }
    public Region Region { get; private set; }
    public PublicHost PublicHost { get; private set; }

    public int PublicPort { get; private set; }
    public string RealityPublicKey { get; private set; }
    public string RealityShortId { get; private set; }
    public string RealitySni { get; private set; }

    public Uri AgentBaseUrl { get; private set; }
    public string AgentTokenEncrypted { get; private set; }
    public ServerStatus Status { get; private set; }
    public int? MaxPeers { get; private set; }

    public HealthSnapshot Health { get; private set; } = default!;
    public MetricsSnapshot Metrics { get; private set; } = default!;
    public BenchmarkSnapshot Benchmark { get; private set; } = default!;

    public DateTime CreatedAt { get; }

    public static Server Register(
        Guid id,
        string name,
        Region region,
        PublicHost host,
        int publicPort,
        string realityPublicKey,
        string realityShortId,
        string realitySni,
        Uri agentBaseUrl,
        string agentTokenEncrypted,
        int? maxPeers)
        => new(
            id,
            name,
            region,
            host,
            publicPort,
            realityPublicKey,
            realityShortId,
            realitySni,
            agentBaseUrl,
            agentTokenEncrypted,
            maxPeers);
}