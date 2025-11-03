using DrakkarVpn.Core.Api.Modules.Servers.Application.Features.Commands.CleanupServerMetricsHistory;
using DrakkarVpn.Core.Api.Modules.Servers.Application.Features.Commands.PullServerMetrics;
using DrakkarVpn.Core.Api.Modules.Servers.Application.Features.Queries.GetServersForHealthPoll;
using MediatR;

public sealed class ServerHealthBackgroundWorker : BackgroundService
{
    private readonly ILogger<ServerHealthBackgroundWorker> _logger;
    private readonly IServiceScopeFactory _scopeFactory;

    private const int DelaySeconds = 10;
    private const int CleanupEveryLoops = 360;
    private int _loop = 0;

    public ServerHealthBackgroundWorker(
        ILogger<ServerHealthBackgroundWorker> logger,
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

                var servers = await mediator.Send(new GetServersForHealthPollRequest(), ct);

                foreach (var s in servers)
                    await mediator.Send(new PullServerMetricsCommand(s.Id), ct);

                _loop++;
                if (_loop % CleanupEveryLoops == 0)
                    await mediator.Send(new CleanupServerMetricsHistoryRequest(TimeSpan.FromHours(48)), ct);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.LogError(ex, "Server health poll failed");
            }

            await Task.Delay(TimeSpan.FromSeconds(DelaySeconds), ct);
        }
    }
}