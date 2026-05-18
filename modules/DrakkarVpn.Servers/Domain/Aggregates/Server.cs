using DrakkarVpn.Core.Api.Modules.Servers.Domain;
using DrakkarVpn.Core.Api.Modules.Servers.Domain.VO;
using DrakkarVpn.Servers.Domain.Abstractions;
using DrakkarVpn.Servers.Domain.Entities;
using DrakkarVpn.Servers.Domain.Enums;
using DrakkarVpn.Servers.Domain.Exceptions;
using DrakkarVpn.Servers.Domain.VO;

namespace DrakkarVpn.Servers.Domain.Aggregates;

public sealed class Server : IAggregateRoot
{
    private const int MinPort = 1;
    private const int MaxPort = 65535;

    private readonly List<ServerTransportProfileActivation> _transportActivations = new();

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
        DesiredTransportActivationId = null;
        DesiredTransportVersion = 0;
        AppliedTransportActivationId = null;
        AppliedTransportVersion = 0;
        AppliedTransportAtUtc = null;
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
    public Guid? DesiredTransportActivationId { get; private set; }
    public long DesiredTransportVersion { get; private set; }
    public Guid? AppliedTransportActivationId { get; private set; }
    public long AppliedTransportVersion { get; private set; }
    public DateTime? AppliedTransportAtUtc { get; private set; }

    public IReadOnlyCollection<ServerTransportProfileActivation> TransportActivations => _transportActivations;

    public static Server Register(
        Guid id,
        string name,
        Region region,
        PublicHost host,
        int publicPort,
        Uri agentBaseUrl,
        string agentTokenEncrypted,
        int? maxPeers)
        => new(id, name, region, host, publicPort, agentBaseUrl, agentTokenEncrypted, maxPeers);

    public void AttachTransportProfile(
        Guid activationId,
        Guid transportProfileId,
        string realityPublicKey,
        int localPriority,
        DateTime utcNow)
    {
        if (activationId == Guid.Empty)
            throw new ArgumentException("ActivationId must not be empty.", nameof(activationId));

        if (transportProfileId == Guid.Empty)
            throw new ArgumentException("TransportProfileId must not be empty.", nameof(transportProfileId));
        
        ArgumentException.ThrowIfNullOrWhiteSpace(realityPublicKey);

        if (_transportActivations.Exists(x => x.TransportProfileId == transportProfileId))
            throw new TransportProfileAlreadyAttachedException(transportProfileId);

        var activation = ServerTransportProfileActivation.Create(
            activationId,
            Id,
            transportProfileId,
            realityPublicKey,
            localPriority,
            utcNow);

        _transportActivations.Add(activation);
    }

    public void ActivateTransportActivation(Guid activationId, DateTimeOffset nowUtc)
    {
        var target = FindActivationOrThrow(activationId);
        
        if (DesiredTransportActivationId == activationId
            && target.Status == TransportActivationStatus.Active)
            return;


        var utcNow = nowUtc.UtcDateTime;

        foreach (var activation in _transportActivations)
        {
            if (activation.Id == activationId)
                continue;

            if (activation.Status == TransportActivationStatus.Active)
                activation.MarkStandby(utcNow);
        }

        target.MarkActive(nowUtc);

        DesiredTransportActivationId = activationId;
        DesiredTransportVersion = checked(DesiredTransportVersion + 1);
    }

    public void MarkTransportApplied(Guid activationId, long version, DateTime utcNow)
    {
        var activation = FindActivationOrThrow(activationId);

        if (activation.Status != TransportActivationStatus.Active)
            throw new TransportActivationNotActiveException(activationId);

        if (version <= AppliedTransportVersion)
            throw new TransportAppliedVersionStaleException(version, AppliedTransportVersion);

        if (version > DesiredTransportVersion)
            throw new TransportAppliedVersionAheadOfDesiredException(version, DesiredTransportVersion);

        AppliedTransportActivationId = activationId;
        AppliedTransportVersion = version;
        AppliedTransportAtUtc = DateTime.SpecifyKind(utcNow, DateTimeKind.Utc);
    }

    public void UpdateTransportActivation(
        Guid activationId,
        string realityPublicKey,
        int localPriority,
        DateTime utcNow)
    {
        var activation = FindActivationOrThrow(activationId);
        activation.Update(realityPublicKey, localPriority, utcNow);
    }

    public void DetachTransportActivation(Guid activationId)
    {
        var activation = FindActivationOrThrow(activationId);

        if (activation.Status == TransportActivationStatus.Active)
            throw new CannotDetachActiveTransportActivationException(activationId);

        _transportActivations.Remove(activation);
    }

    private ServerTransportProfileActivation FindActivationOrThrow(Guid activationId)
    {
        var activation = _transportActivations.Find(x => x.Id == activationId);
        if (activation is null)
            throw new TransportActivationNotAttachedException(activationId);

        return activation;
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
            throw new ArgumentException("Value must not be empty.", paramName);

        var normalized = value.Trim();
        if (maxLength.HasValue && normalized.Length > maxLength.Value)
            throw new ArgumentOutOfRangeException(paramName, $"Maximum length is {maxLength.Value}.");

        return normalized;
    }
}