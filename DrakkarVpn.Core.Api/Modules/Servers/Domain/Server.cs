using DrakkarVpn.Core.Api.Modules.Servers.Application.Features.Commands.EvaluateServerHealth;
using DrakkarVpn.Core.Api.Modules.Servers.Domain.Abstractions;
using DrakkarVpn.Core.Api.Modules.Servers.Domain.VO;

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
        Metrics = MetricsSnapshot.Default;
        Benchmark = BenchmarkSnapshot.Empty;
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
    public MetricsSnapshot Metrics { get; private set; } = default!;
    public BenchmarkSnapshot Benchmark { get; private set; } = default!;

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
        if (maxPeers is < 0)
            throw new ArgumentOutOfRangeException(nameof(maxPeers));
        MaxPeers = maxPeers;
    }

    public void UpdateHealth(bool reachable, int peersActive)
    {
        if (peersActive < 0)
            peersActive = 0;

        Health = new HealthSnapshot(reachable, peersActive, DateTime.UtcNow);
    }

    public void UpdateMetrics(
        long trafficRxBytes,
        long trafficTxBytes,
        double vpnSpeedMbps,
        double infraLatencyMs)
    {
        Metrics = new MetricsSnapshot(
            trafficRxBytes,
            trafficTxBytes,
            vpnSpeedMbps,
            infraLatencyMs,
            DateTime.UtcNow
        );
    }

    public void UpdateBenchmark(double maxSpeedMbps)
    {
        Benchmark = new BenchmarkSnapshot(maxSpeedMbps, DateTime.UtcNow);
    }

    public bool IsEligible() =>
        Status == ServerStatus.Enabled &&
        Health.Reachable &&
        (MaxPeers is null || Health.PeersActive < MaxPeers);

    public bool IsHealthChanged(EvaluateServerHealthRequest req) =>
        Health.Reachable != req.Reachable ||
        Health.PeersActive != req.PeersActive;

    public bool IsMetricsChanged(EvaluateServerHealthRequest req) =>
        Metrics.TrafficRxBytes != req.TrafficRxBytes ||
        Metrics.TrafficTxBytes != req.TrafficTxBytes ||
        Metrics.VpnSpeedMbps != req.VpnSpeedMbps ||
        Metrics.InfraLatencyMs != req.InfraLatencyMs;

    public bool IsSameState(EvaluateServerHealthRequest req) =>
        !IsHealthChanged(req) && !IsMetricsChanged(req);
}
