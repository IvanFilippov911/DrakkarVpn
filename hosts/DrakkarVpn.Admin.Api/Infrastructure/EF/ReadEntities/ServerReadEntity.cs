using DrakkarVpn.Core.Api.Modules.Servers.Domain;

namespace DrakkarVpn.Core.Api.Modules.Admin.Infrastructure.EF.Entities;

/// <summary>
/// Read-oriented server model for admin read-side. Maps to "servers" table.
/// Intentionally avoids write-side aggregate/value objects to keep EF mapping stable.
/// </summary>
public sealed class ServerReadEntity
{
    public Guid Id { get; init; }
    public string Name { get; init; } = default!;
    public string Region { get; init; } = default!;
    public ServerStatus Status { get; init; }
    public string PublicHost { get; init; } = default!;
    public int? MaxPeers { get; init; }

    public bool HealthReachable { get; init; }
    public int HealthPeersActive { get; init; }
    public DateTime HealthUpdatedAtUtc { get; init; }

    public long? MetricsRxBytes { get; init; }
    public long? MetricsTxBytes { get; init; }
    public double? MetricsInfraLatencyMs { get; init; }
    public double? MetricsVpnSpeedMbps { get; init; }
    public DateTime MetricsUpdatedAtUtc { get; init; }

    public double BenchmarkMaxSpeedMbps { get; init; }
    public DateTime BenchmarkMeasuredAt { get; init; }

    public DateTime CreatedAt { get; init; }
}

