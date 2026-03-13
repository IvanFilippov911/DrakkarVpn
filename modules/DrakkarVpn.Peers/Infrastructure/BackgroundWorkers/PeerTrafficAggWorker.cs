using DrakkarVpn.Core.Api.Modules.Peers.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Peers.Infrastructure.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DrakkarVpn.Core.Api.Modules.Peers.Infrastructure.BackgroundWorkers;

public sealed class PeerTrafficAggWorker : BackgroundService
{
    private readonly IServiceScopeFactory           _scopeFactory;
    private readonly ILogger<PeerTrafficAggWorker> _logger;

    private static readonly TimeSpan Interval = TimeSpan.FromMinutes(1);

    public PeerTrafficAggWorker(
        IServiceScopeFactory scopeFactory,
        ILogger<PeerTrafficAggWorker> logger)
    {
        _scopeFactory = scopeFactory;
        _logger       = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("PeerTrafficAggWorker started");
        
        await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();

                var history = scope.ServiceProvider.GetRequiredService<IPeerMetricsHistoryRepository>();
                var aggRepo = scope.ServiceProvider.GetRequiredService<IPeerTrafficAggRepository>();
                var peersUow = scope.ServiceProvider.GetRequiredService<IPeersUnitOfWork>();

                await RecalculateAsync(history, aggRepo, peersUow, stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "PeerTrafficAggWorker: error on recalc");
            }

            try
            {
                await Task.Delay(Interval, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
        }

        _logger.LogInformation("PeerTrafficAggWorker stopped");
    }

    private async Task RecalculateAsync(
        IPeerMetricsHistoryRepository history,
        IPeerTrafficAggRepository aggRepo,
        IPeersUnitOfWork peersUow,
        CancellationToken ct)
    {
        var nowUtc  = DateTime.UtcNow;
        var from1h  = nowUtc.AddHours(-1);
        var from24h = nowUtc.AddHours(-24);

        var rows1h  = await history.GetTrafficAggForWindowAsync(from1h,  nowUtc, ct);
        var rows24h = await history.GetTrafficAggForWindowAsync(from24h, nowUtc, ct);

        var map24h = rows24h.ToDictionary(x => x.PeerId, x => x.TrafficBytes);

        var result = new List<PeerTrafficAgg>(rows1h.Count);

        foreach (var r1 in rows1h)
        {
            map24h.TryGetValue(r1.PeerId, out var bytes24h);

            result.Add(new PeerTrafficAgg
            {
                PeerId        = r1.PeerId,
                Last1hBytes   = r1.TrafficBytes,
                Last24hBytes  = bytes24h,
                UpdatedAtUtc  = nowUtc
            });
        }

        await aggRepo.UpsertBatchAsync(result, ct);
        await peersUow.SaveChangesAsync(ct);
    }
}