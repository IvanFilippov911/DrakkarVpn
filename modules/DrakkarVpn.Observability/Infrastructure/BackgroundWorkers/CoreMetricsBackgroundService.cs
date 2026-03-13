
using System.Text.Json;
using Microsoft.Extensions.Hosting;
using StackExchange.Redis;

namespace DrakkarVpn.Core.Api.Diagnostics;

public sealed class CoreMetricsBackgroundService : BackgroundService
{
    private readonly CoreRequestMetricsBuffer _buffer;
    private readonly IDatabase _redis;
    private readonly TimeSpan _interval = TimeSpan.FromSeconds(5);
    private const string RedisKey = "drakkar:core:metrics:summary";
    private const string HistoryKey = "drakkar:core:metrics:history";
    private const int HistorySize = 36;

    public CoreMetricsBackgroundService(
        CoreRequestMetricsBuffer buffer,
        IConnectionMultiplexer redis)
    {
        _buffer = buffer;
        _redis = redis.GetDatabase();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(_interval, stoppingToken);

            var snapshot = _buffer.SnapshotAndReset();

            if (snapshot.TotalRequests == 0 && snapshot.TotalErrors == 0 &&
                snapshot.ErrorsByArea.Count == 0)
                continue;

            var windowSeconds = (snapshot.WindowEndUtc - snapshot.WindowStartUtc).TotalSeconds;
            if (windowSeconds <= 0) windowSeconds = _interval.TotalSeconds;

            var rps = snapshot.TotalRequests / windowSeconds;

            var entries = new List<HashEntry>
            {
                new("WindowStartUtc", snapshot.WindowStartUtc.ToString("O")),
                new("WindowEndUtc",   snapshot.WindowEndUtc.ToString("O")),
                new("WindowSeconds",  Math.Round(windowSeconds, 3)),
                new("TotalRequests",  snapshot.TotalRequests),
                new("TotalErrors",    snapshot.TotalErrors),
                new("Rps",            Math.Round(rps, 2)),
                new("AvgLatencyMs",   Math.Round(snapshot.AvgLatencyMs ?? 0, 2)),
                new("ErrorRatePct",   Math.Round(snapshot.ErrorRatePercent, 4))
            };

            foreach (var kvp in snapshot.ErrorsByArea)
                entries.Add(new HashEntry($"Errors.{kvp.Key}", kvp.Value));

            await _redis.HashSetAsync(RedisKey, entries.ToArray());
            await _redis.KeyExpireAsync(RedisKey, TimeSpan.FromSeconds(120));
            
            var entry = new CoreMetricsHistoryEntry(
                TimestampUtc: snapshot.WindowEndUtc,
                Rps: Math.Round(rps, 2),
                AvgLatencyMs: Math.Round(snapshot.AvgLatencyMs ?? 0, 2),
                ErrorRatePct: Math.Round(snapshot.ErrorRatePercent, 4),
                TotalErrors: snapshot.TotalErrors,
                ErrorsByArea: snapshot.ErrorsByArea.Count > 0 ? snapshot.ErrorsByArea : null
            );

            var json = JsonSerializer.Serialize(entry);

            await _redis.ListRightPushAsync(HistoryKey, json);
            await _redis.ListTrimAsync(HistoryKey, 0, 35); 
            await _redis.KeyExpireAsync(HistoryKey, TimeSpan.FromHours(1));
        }
    }
}