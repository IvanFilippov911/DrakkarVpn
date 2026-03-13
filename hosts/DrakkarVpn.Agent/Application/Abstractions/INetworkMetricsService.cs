using DrakkarVpn.Agent.Application.Services;

namespace DrakkarVpn.Agent.Application.Abstractions;

public interface INetworkMetricsService
{
    Task<(long RxBytes, long TxBytes)> GetTrafficAsync(string iface, CancellationToken ct = default);
    double CalculateSpeedMbps(string iface, long rxBytes, long txBytes);
    Task<double> MeasureInfraLatencyAsync(CancellationToken ct);
}