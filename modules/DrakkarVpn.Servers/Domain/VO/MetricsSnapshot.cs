using Microsoft.EntityFrameworkCore;

namespace DrakkarVpn.Core.Api.Modules.Servers.Domain.VO;

[Owned]
public sealed class MetricsSnapshot
{
    public long TrafficRxBytes { get; private set; }
    public long TrafficTxBytes { get; private set; }
    public double VpnSpeedMbps { get; private set; }
    public double InfraLatencyMs { get; private set; }
    public DateTime LastMetricsAtUtc { get; private set; }

    private MetricsSnapshot() { }

    public MetricsSnapshot(
        long trafficRxBytes,
        long trafficTxBytes,
        double vpnSpeedMbps,
        double infraLatencyMs,
        DateTime lastMetricsAtUtc)
    {
        TrafficRxBytes = trafficRxBytes;
        TrafficTxBytes = trafficTxBytes;
        VpnSpeedMbps = vpnSpeedMbps;
        InfraLatencyMs = infraLatencyMs;
        LastMetricsAtUtc = lastMetricsAtUtc;
    }

    public static readonly MetricsSnapshot Default = new(
        trafficRxBytes: 0,
        trafficTxBytes: 0,
        vpnSpeedMbps: 0,
        infraLatencyMs: 0,
        lastMetricsAtUtc: DateTime.MinValue
    );
}