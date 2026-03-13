using System.Collections.Concurrent;

namespace DrakkarVpn.Core.Api.Diagnostics;

public sealed class CoreRequestMetricsBuffer : ICoreRequestMetricsSink
{
    private sealed class Counter
    {
        public long SuccessCount;
        public long ErrorCount;
        public long TotalElapsedMs;
    }

    private readonly ConcurrentDictionary<string, Counter> _counters = new();
    private readonly ConcurrentDictionary<string, long> _errorsByArea = new();

    private DateTime _windowStartUtc = DateTime.UtcNow;

    public void TrackSuccess(string command, long elapsedMs)
    {
        var counter = _counters.GetOrAdd(command, _ => new Counter());
        Interlocked.Increment(ref counter.SuccessCount);
        Interlocked.Add(ref counter.TotalElapsedMs, elapsedMs);
    }

    public void TrackError(string command, long elapsedMs, string? area = null)
    {
        var counter = _counters.GetOrAdd(command, _ => new Counter());
        Interlocked.Increment(ref counter.ErrorCount);
        Interlocked.Add(ref counter.TotalElapsedMs, elapsedMs);

        if (!string.IsNullOrWhiteSpace(area))
        {
            _errorsByArea.AddOrUpdate(
                area,
                1,
                (_, old) => old + 1
            );
        }
    }

    public CoreRequestMetricsSnapshot SnapshotAndReset()
    {
        var windowEnd = DateTime.UtcNow;
        
        var perCommand = new Dictionary<string, CoreCommandMetrics>(_counters.Count);
        foreach (var kvp in _counters)
        {
            var c = kvp.Value;
            var success = Interlocked.Exchange(ref c.SuccessCount, 0);
            var error   = Interlocked.Exchange(ref c.ErrorCount, 0);
            var totalMs = Interlocked.Exchange(ref c.TotalElapsedMs, 0);

            if (success == 0 && error == 0 && totalMs == 0)
                continue;

            perCommand[kvp.Key] = new CoreCommandMetrics(
                SuccessCount:  success,
                ErrorCount:    error,
                TotalElapsedMs: totalMs
            );
        }
        
        var errorsByArea = new Dictionary<string, long>(_errorsByArea.Count);
        foreach (var kvp in _errorsByArea.ToArray())
        {
            errorsByArea[kvp.Key] = kvp.Value;
            _errorsByArea[kvp.Key] = 0; 
        }

        var snapshot = new CoreRequestMetricsSnapshot(
            WindowStartUtc: _windowStartUtc,
            WindowEndUtc:   windowEnd,
            PerCommand:     perCommand,
            ErrorsByArea:   errorsByArea
        );

        _windowStartUtc = windowEnd;
        return snapshot;
    }
}