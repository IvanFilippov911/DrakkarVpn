using DrakkarVpn.Core.Api.Modules.Servers.Domain.Abstractions;
using DrakkarVpn.Core.Api.Modules.Servers.Domain.VO;
using DrakkarVpn.Servers.Domain.Aggregates;
using DrakkarVpn.Servers.Domain.Enums;
using DrakkarVpn.Servers.Domain.Inputs;
using DrakkarVpn.Servers.Domain.Policies;

namespace DrakkarVpn.Core.Api.Modules.Servers.Domain;

public sealed class Server : IAggregateRoot
{
    private const int MinPort = 1;
    private const int MaxPort = 65535;
    private const int DisableFailuresThreshold = 3;
    private const double DrainingPeersThreshold = 0.95;

    private readonly List<ServerTransportProfile> _transportProfiles = new();

    private Server() { }

    private Server(
        Guid id,
        string name,
        Region region,
        PublicHost host,
        int publicPort,
        Uri agentBaseUrl,
        string agentTokenEncrypted,
        int? maxPeers)
    {
        Id = id;
        Name = NormalizeRequired(name, nameof(name), 100);
        Region = region;
        PublicHost = host;

        PublicPort = ValidatePort(publicPort);

        AgentBaseUrl = agentBaseUrl ?? throw new ArgumentNullException(nameof(agentBaseUrl));
        AgentTokenEncrypted = NormalizeRequired(agentTokenEncrypted, nameof(agentTokenEncrypted));

        MaxPeers = ValidateMaxPeers(maxPeers);
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

    public Uri AgentBaseUrl { get; private set; }
    public string AgentTokenEncrypted { get; private set; }
    public ServerStatus Status { get; private set; }
    public int? MaxPeers { get; private set; }

    public HealthSnapshot Health { get; private set; } = default!;
    public MetricsSnapshot Metrics { get; private set; } = default!;
    public BenchmarkSnapshot Benchmark { get; private set; } = default!;

    public DateTime CreatedAt { get; }

    public IReadOnlyCollection<ServerTransportProfile> TransportProfiles => _transportProfiles.AsReadOnly();

    public static Server Register(
        Guid id,
        string name,
        Region region,
        PublicHost host,
        int publicPort,
        Uri agentBaseUrl,
        string agentTokenEncrypted,
        int? maxPeers)
        => new(
            id,
            name,
            region,
            host,
            publicPort,
            agentBaseUrl,
            agentTokenEncrypted,
            maxPeers);
    
    
    public void AddTransportProfile(ServerTransportProfile profile)
    {
        ArgumentNullException.ThrowIfNull(profile);

        if (profile.ServerId != Id)
            throw new ArgumentException("Profile belongs to a different server.", nameof(profile));

        if (_transportProfiles.Exists(p => p.Id == profile.Id))
            throw new InvalidOperationException("A transport profile with the same id already exists.");

        if (profile.Status == TransportProfileStatus.Active
            && _transportProfiles.Exists(p => p.Status == TransportProfileStatus.Active))
            throw new InvalidOperationException("Cannot add an active profile while another profile is already active.");

        _transportProfiles.Add(profile);
    }
    
    
    public void ActivateTransportProfile(Guid profileId, DateTimeOffset nowUtc)
    {
        var profile = _transportProfiles.Find(p => p.Id == profileId)
            ?? throw new InvalidOperationException("Transport profile was not found.");

        var utcNow = nowUtc.UtcDateTime;

        foreach (var p in _transportProfiles)
        {
            if (p.Status != TransportProfileStatus.Active || p.Id == profileId)
                continue;

            p.MarkStandby(utcNow);
        }

        profile.MarkActive(nowUtc);
    }
    

    private static int ValidatePort(int publicPort)
    {
        if (publicPort < MinPort || publicPort > MaxPort)
            throw new ArgumentOutOfRangeException(nameof(publicPort), "Public port must be in range 1..65535.");

        return publicPort;
    }

    private static int? ValidateMaxPeers(int? maxPeers)
    {
        if (maxPeers.HasValue && maxPeers.Value <= 0)
            throw new ArgumentOutOfRangeException(nameof(maxPeers), "MaxPeers must be greater than zero.");

        return maxPeers;
    }

    private static string NormalizeRequired(string value, string paramName, int? maxLength = null)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentNullException(paramName);

        var normalized = value.Trim();
        if (maxLength.HasValue && normalized.Length > maxLength.Value)
            throw new ArgumentOutOfRangeException(paramName, $"Maximum length is {maxLength.Value}.");

        return normalized;
    }
}