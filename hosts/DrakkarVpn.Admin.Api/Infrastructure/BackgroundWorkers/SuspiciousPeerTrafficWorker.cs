/*using System.Collections.Concurrent;
using System.Diagnostics;
using DrakkarVpn.Core.Api.Modules.Peers.Application.Abstractions;
using DrakkarVpn.Observability.Application.Features.Services.Alerts.Factories.PeerTraffic;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Admin.Infrastructure.BackgroundWorkers;

public sealed class SuspiciousPeerTrafficWorker : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<SuspiciousPeerTrafficWorker> _logger;

    private readonly TimeSpan _interval    = TimeSpan.FromMinutes(1);
    private readonly TimeSpan _windowSize  = TimeSpan.FromMinutes(10);
    private readonly TimeSpan _suppressFor = TimeSpan.FromMinutes(30);
    
    private const int MaxAlertsPerWindow = 30;
    
    private readonly ConcurrentDictionary<Guid, DateTime> _lastAlertAt = new();

    public SuspiciousPeerTrafficWorker(
        IServiceScopeFactory scopeFactory,
        ILogger<SuspiciousPeerTrafficWorker> logger)
    {
        _scopeFactory = scopeFactory;
        _logger       = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("SuspiciousPeerTrafficWorker started");

        while (!stoppingToken.IsCancellationRequested)
        {
            var sw = Stopwatch.StartNew();
            int snapshotsCount   = 0;
            int generatedAlerts  = 0;
            int sentAlerts       = 0;

            try
            {
                var now  = DateTime.UtcNow;
                var from = now - _windowSize;

                using (var scope = _scopeFactory.CreateScope())
                {
                    var historyRepo  = scope.ServiceProvider.GetRequiredService<IPeerMetricsHistoryRepository>();
                    var alertFactory = scope.ServiceProvider.GetRequiredService<IPeerTrafficAlertFactory>();
                    var mediator     = scope.ServiceProvider.GetRequiredService<IMediator>();

                    var snapshots = await historyRepo
                        .GetRecentTrafficSummaryAsync(from, now, stoppingToken);

                    snapshotsCount = snapshots.Count;

                    var allAlerts = new List<CreateCoreAlertCommand>();

                    foreach (var snap in snapshots)
                    {
                        if (IsSuppressed(snap.PeerId, now))
                            continue;

                        var alerts = alertFactory.Build(snap);
                        if (alerts.Count == 0)
                            continue;

                        generatedAlerts += alerts.Count;
                        
                        _lastAlertAt[snap.PeerId] = now;

                        allAlerts.AddRange(alerts);
                    }

                    if (allAlerts.Count > 0)
                    {
                        var limited = LimitAlertsPerWindow(allAlerts, MaxAlertsPerWindow);
                        sentAlerts = limited.Count;
                        
                        await mediator.Send(new CreateCoreAlertsBatchCommand(limited), stoppingToken);
                    }
                }
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in SuspiciousPeerTrafficWorker");
            }
            finally
            {
                sw.Stop();
                _logger.LogInformation(
                    "SuspiciousPeerTrafficWorker cycle finished: duration_ms={DurationMs}, snapshots={Snapshots}, alerts_generated={Generated}, alerts_sent={Sent}",
                    sw.ElapsedMilliseconds,
                    snapshotsCount,
                    generatedAlerts,
                    sentAlerts);
            }

            await Task.Delay(_interval, stoppingToken);
        }

        _logger.LogInformation("SuspiciousPeerTrafficWorker stopped");
    }

    private bool IsSuppressed(Guid peerId, DateTime nowUtc)
    {
        if (!_lastAlertAt.TryGetValue(peerId, out var last))
            return false;

        return (nowUtc - last) < _suppressFor;
    }

    private static IReadOnlyList<CreateCoreAlertCommand> LimitAlertsPerWindow(
        IReadOnlyList<CreateCoreAlertCommand> alerts,
        int limit)
    {
        if (alerts.Count <= limit)
            return alerts;

        static int SeverityScore(string? severity) => severity switch
        {
            "Critical" => 3,
            "Warning"  => 2,
            "Info"     => 1,
            _          => 0
        };

        return alerts
            .OrderByDescending(a => SeverityScore(a.Severity))
            .ThenBy(a => a.Code)
            .Take(limit)
            .ToList();
    }
} */