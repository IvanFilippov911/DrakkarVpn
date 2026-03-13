using DrakkarVpn.Core.Api.Modules.Servers.Application.Handlers.RunServersPolling;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DrakkarVpn.Core.Api.Modules.Orchestrator.Infrastructure.BackgroundWorkers;

public sealed class ServersPollingBackgroundWorker : BackgroundService
{
    private readonly ILogger<ServersPollingBackgroundWorker> _logger;
    private readonly IServiceScopeFactory _scopeFactory;

    private static readonly TimeSpan Delay = TimeSpan.FromSeconds(10);

    public ServersPollingBackgroundWorker(
        ILogger<ServersPollingBackgroundWorker> logger,
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
                
                var cmd = new RunServersPollingCommand(
                    BatchSize: 10,
                    LeaseDuration: TimeSpan.FromSeconds(20),
                    StuckTimeout: TimeSpan.FromMinutes(2),
                    HttpConcurrency: 5);

                await mediator.Send(cmd, ct);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.LogError(ex, "Servers polling worker failed");
            }

            await Task.Delay(Delay, ct);
        }
    }
}