using DrakkarVpn.Core.Api.Modules.Servers.Application.Features.Commands.CleanupServerMetricsHistory;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DrakkarVpn.Core.Api.Modules.Orchestrator.Infrastructure.BackgroundWorkers;

public sealed class ServerMetricsHistoryCleanupWorker : BackgroundService
{
    private readonly ILogger<ServerMetricsHistoryCleanupWorker> _log;
    private readonly IServiceScopeFactory _scopeFactory;

    private static readonly TimeSpan Delay = TimeSpan.FromHours(1);

    public ServerMetricsHistoryCleanupWorker(
        ILogger<ServerMetricsHistoryCleanupWorker> log,
        IServiceScopeFactory scopeFactory)
    {
        _log = log;
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

                await mediator.Send(
                    new CleanupServerMetricsHistoryRequest(TimeSpan.FromHours(48)),
                    ct);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _log.LogError(ex, "Metrics history cleanup failed");
            }

            await Task.Delay(Delay, ct);
        }
    }
}