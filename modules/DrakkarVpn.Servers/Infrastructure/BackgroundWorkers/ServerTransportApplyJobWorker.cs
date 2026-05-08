using DrakkarVpn.Servers.Application.Abstractions.Services;
using DrakkarVpn.Servers.Application.Handlers.RunServerTransportApplyJob;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DrakkarVpn.Core.Api.Modules.Orchestrator.Infrastructure.BackgroundWorkers;

public sealed class ServerTransportApplyJobWorker : BackgroundService
{
    private static readonly TimeSpan Interval = TimeSpan.FromSeconds(2);
    private static readonly TimeSpan LeaseDuration = TimeSpan.FromSeconds(30);
    private static readonly TimeSpan ErrorBackoff = TimeSpan.FromSeconds(1);
    private const int BatchSize = 20;
    private const int StartupJitterMs = 200;

    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<ServerTransportApplyJobWorker> _log;
    private readonly string _leaseOwner = $"{Environment.MachineName}:{Environment.ProcessId}";

    public ServerTransportApplyJobWorker(
        IServiceScopeFactory scopeFactory,
        ILogger<ServerTransportApplyJobWorker> log)
    {
        _scopeFactory = scopeFactory;
        _log = log;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _log.LogInformation(
            "ServerTransportApplyJobWorker started. Interval={IntervalMs} BatchSize={BatchSize}",
            (int)Interval.TotalMilliseconds,
            BatchSize);

        if (StartupJitterMs > 0)
        {
            var jitter = Random.Shared.Next(0, StartupJitterMs);
            await Task.Delay(TimeSpan.FromMilliseconds(jitter), stoppingToken);
        }

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();

                var jobsService = scope.ServiceProvider.GetRequiredService<IServerTransportApplyJobService>();
                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

                var jobs = await jobsService.AcquireBatchAsync(
                    take: BatchSize,
                    lease: LeaseDuration,
                    utcNow: DateTime.UtcNow,
                    leaseOwner: _leaseOwner,
                    ct: stoppingToken);

                if (jobs.Count > 0)
                {
                    await mediator.Send(
                        new RunServerTransportApplyJobRequest(jobs, _leaseOwner),
                        stoppingToken);
                }
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "ServerTransportApplyJobWorker tick failed");

                if (ErrorBackoff > TimeSpan.Zero)
                    await Task.Delay(ErrorBackoff, stoppingToken);
            }

            await Task.Delay(Interval, stoppingToken);
        }

        _log.LogInformation("ServerTransportApplyJobWorker stopped");
    }
}

