using DrakkarVpn.Core.Api.Modules.Peers.Application.Handlers.PeerCreate;
using DrakkarVpn.Core.Api.Modules.Peers.Application.Options;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DrakkarVpn.Peers.Infrastructure.BackgroundWorkers;

public sealed class PeerProvisioningWorker : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<PeerProvisioningWorker> _log;
    private readonly PeerProvisioningWorkerOptions _opt;

    public PeerProvisioningWorker(
        IServiceScopeFactory scopeFactory,
        IOptions<PeerProvisioningWorkerOptions> options,
        ILogger<PeerProvisioningWorker> log)
    {
        _scopeFactory = scopeFactory;
        _log = log;
        _opt = options.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _log.LogInformation(
            "PeerProvisioningWorker started. Interval={IntervalMs}ms BatchSize={BatchSize} Concurrency={Concurrency}",
            (int)_opt.Interval.TotalMilliseconds,
            _opt.BatchSize,
            _opt.HttpConcurrency);

        if (_opt.StartupJitterMs > 0)
        {
            var jitter = Random.Shared.Next(0, _opt.StartupJitterMs);
            await Task.Delay(TimeSpan.FromMilliseconds(jitter), stoppingToken);
        }

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();

                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

                var cmd = new PeerCreateCommand(
                    BatchSize: _opt.BatchSize,
                    LeaseDuration: _opt.LeaseDuration,
                    StuckTimeout: _opt.StuckTimeout,
                    HttpConcurrency: _opt.HttpConcurrency
                );

                await mediator.Send(cmd, stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "PeerProvisioningWorker tick failed");

                if (_opt.ErrorBackoff > TimeSpan.Zero)
                    await Task.Delay(_opt.ErrorBackoff, stoppingToken);
            }

            await Task.Delay(_opt.Interval, stoppingToken);
        }

        _log.LogInformation("PeerProvisioningWorker stopped");
    }
}