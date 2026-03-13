namespace DrakkarVpn.Core.Api.Modules.Servers.Infrastructure.Entities;

public sealed class ServerMetricsHistory
{
    public DateTime PeriodStartUtc { get; set; }   
    public Guid ServerId { get; set; }
    public bool Reachable { get; set; }
    public long TrafficRxDeltaBytes { get; set; }   
    public long TrafficTxDeltaBytes { get; set; }
    public decimal VpnSpeedMbps { get; set; }
    public decimal InfraLatencyMs { get; set; }
}
