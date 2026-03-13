using DrakkarVpn.Core.Api.Modules.Peers.Application.Features.Commands.CleanupPeerMetricsHistory;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DrakkarVpn.Core.Api.Modules.Peers.Infrastructure.BackgroundWorkers;
public sealed class PeerMetricsBackgroundWorker : BackgroundService
{
    private readonly ILogger<PeerMetricsBackgroundWorker> _logger;
    private readonly IServiceScopeFactory _scopeFactory;

    private const int DelaySeconds = 10;
    private const int CleanupEveryLoops = 360;
    private int _loop = 0;

    public PeerMetricsBackgroundWorker(
        ILogger<PeerMetricsBackgroundWorker> logger,
        IServiceScopeFactory scopeFactory)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

                //var servers = await mediator.Send(new GetServersForHealthPollRequest(), ct);

                //foreach (var s in servers)
                //    await mediator.Send(new PullServerPeerMetricsCommand(s.Id), ct);

                _loop++;
                if (_loop % CleanupEveryLoops == 0)
                {
                    await mediator.Send(
                        new CleanupPeerMetricsHistoryCommand(TimeSpan.FromHours(24)),
                        ct);
                    _logger.LogInformation("PeerMetricsHistory: cleaned entries older than 24h");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "peer-metrics poll failed");
            }

            await Task.Delay(TimeSpan.FromSeconds(DelaySeconds), ct);
        }
    }
}