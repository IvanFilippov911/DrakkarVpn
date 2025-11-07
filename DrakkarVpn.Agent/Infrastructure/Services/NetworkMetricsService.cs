using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using DrakkarVpn.Agent.Application.Abstractions;

namespace DrakkarVpn.Agent.Application.Services;

public sealed class NetworkMetricsService : INetworkMetricsService
{
    private readonly ILogger<NetworkMetricsService> _logger;
    private readonly ConcurrentDictionary<string, Sample> _lastSamples = new();
    
    private const string CoreApiUrl = "https://google.com";
    private const double BitsInByte = 8d;
    private const double MegabitDivisor = 1_000_000d;
    private const int SpeedPrecision = 2;
    private const double MinIntervalSeconds = 1.0;
    private const int MaxRetries = 3;
    private const int RetryDelayMs = 5000;
    
    private sealed record Sample(long Rx, long Tx, DateTime Timestamp, double SpeedMbps = 0);

    public NetworkMetricsService(ILogger<NetworkMetricsService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<(long RxBytes, long TxBytes)> GetTrafficAsync(string iface, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(iface))
            throw new ArgumentException("Interface cannot be empty or null.", nameof(iface));

        int retries = 0;
        while (retries < MaxRetries)
        {
            try
            {
                var networkInterface = NetworkInterface.GetAllNetworkInterfaces()
                    .FirstOrDefault(ni => ni.Name.Equals(iface, StringComparison.OrdinalIgnoreCase));
                
                if (networkInterface == null || networkInterface.OperationalStatus != OperationalStatus.Up)
                {
                    _logger.LogWarning("Network interface {iface} not found or not operational. Retry {retry}/{max}.", iface, retries + 1, MaxRetries);
                    retries++;
                    await Task.Delay(RetryDelayMs, ct);
                    continue;
                }

                var stats = networkInterface.GetIPStatistics();
                var rxBytes = stats.BytesReceived;
                var txBytes = stats.BytesSent;

                _logger.LogDebug("Retrieved traffic for {iface}: Rx={rxBytes}, Tx={txBytes}", iface, rxBytes, txBytes);
                return (rxBytes, txBytes);
            }
            catch (Exception ex) when (ex is NetworkInformationException or PlatformNotSupportedException or InvalidOperationException)
            {
                _logger.LogWarning(ex, "Failed to read network stats for {iface}. Retry {retry}/{max}.", iface, retries + 1, MaxRetries);
                retries++;
                await Task.Delay(RetryDelayMs, ct);
            }
        }

        throw new InvalidOperationException($"Failed to retrieve network stats for interface '{iface}' after {MaxRetries} retries.");
    }

    
    public double CalculateSpeedMbps(string iface, long rxBytes, long txBytes)
    {
        if (string.IsNullOrWhiteSpace(iface))
            throw new ArgumentException("Interface cannot be empty or null.", nameof(iface));
        if (rxBytes < 0 || txBytes < 0)
            throw new ArgumentException("Byte values cannot be negative.", nameof(rxBytes));

        var now = DateTime.UtcNow;

        try
        {
            var sample = _lastSamples.AddOrUpdate(
                iface,
                _ => new Sample(rxBytes, txBytes, now),
                (_, prev) =>
                {
                    var timeDiff = (now - prev.Timestamp).TotalSeconds;
                    if (timeDiff < MinIntervalSeconds) return prev;

                    var deltaRx = CalculateDelta(rxBytes, prev.Rx);
                    var deltaTx = CalculateDelta(txBytes, prev.Tx);
                    var deltaBytes = Math.Max(0, deltaRx + deltaTx);
                    var deltaSec = Math.Max(MinIntervalSeconds, timeDiff);

                    var speed = (deltaBytes * BitsInByte) / MegabitDivisor / deltaSec;
                    return new Sample(rxBytes, txBytes, now, Math.Round(speed, SpeedPrecision));
                });

            _logger.LogDebug("Calculated speed for {iface}: {speed} Mbps", iface, sample.SpeedMbps);
            return sample.SpeedMbps;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to calculate speed for {iface}.", iface);
            return 0;
        }
    }
    
    private long CalculateDelta(long current, long previous)
    {
        if (current >= previous)
            return current - previous;

        _logger.LogDebug("Counter overflow detected: current={current}, previous={previous}", current, previous);
        long delta = current + (long.MaxValue - previous) + 1;
        return delta > 0 ? delta : 0;
    }
    

    public async Task<double> MeasureInfraLatencyAsync(CancellationToken ct)
    {
        var sw = Stopwatch.StartNew();
        try
        {
            using var http = new HttpClient { Timeout = TimeSpan.FromSeconds(5) };
            var resp = await http.GetAsync(CoreApiUrl, ct);
            resp.EnsureSuccessStatusCode();
            sw.Stop();
            return sw.Elapsed.TotalMilliseconds;
        }
        catch
        {
            return -1;
        }
    }
    
}