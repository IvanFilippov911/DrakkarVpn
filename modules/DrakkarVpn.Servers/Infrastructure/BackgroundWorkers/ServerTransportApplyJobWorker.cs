using DrakkarVpn.Servers.Application.Abstractions.Services;
using DrakkarVpn.Servers.Application.Handlers.RunServerTransportApplyJob;
using DrakkarVpn.Servers.Application.Options;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DrakkarVpn.Servers.Infrastructure.BackgroundWorkers;

public sealed class ServerTransportApplyJobWorker : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<ServerTransportApplyJobWorker> _log;
    private readonly ServerTransportApplyJobOptions _opt;
    private readonly string _leaseOwner = $"{Environment.MachineName}:{Environment.ProcessId}";

    public ServerTransportApplyJobWorker(
        IServiceScopeFactory scopeFactory,
        IOptions<ServerTransportApplyJobOptions> options,
        ILogger<ServerTransportApplyJobWorker> log)
    {
        _scopeFactory = scopeFactory;
        _log = log;
        
        ArgumentNullException.ThrowIfNull(options);
        _opt = options.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _log.LogInformation(
            "ServerTransportApplyJobWorker started. Interval={IntervalMs} BatchSize={BatchSize}",
            (int)_opt.Interval.TotalMilliseconds,
            _opt.BatchSize);

        try
        {
            await ApplyStartupJitterAsync(stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await ProcessTickAsync(stoppingToken);
                    await DelayOrStopAsync(_opt.Interval, stoppingToken);
                }

                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    break;
                }

                catch (Exception ex)
                {
                    _log.LogError(
                        ex,
                        "ServerTransportApplyJobWorker tick failed. LeaseOwner={LeaseOwner}",
                        _leaseOwner);
                    await DelayOrStopAsync(_opt.ErrorBackoff, stoppingToken);
                }
            }
        }
        finally
        {
            _log.LogInformation(
                "ServerTransportApplyJobWorker stopped. LeaseOwner={LeaseOwner}",
                _leaseOwner);
        }
    }
    
    private async Task ProcessTickAsync(CancellationToken ct)
    {
        using var scope = _scopeFactory.CreateScope();

        var jobsService = scope.ServiceProvider.GetRequiredService<IServerTransportApplyJobService>();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        var jobs = await jobsService.AcquireBatchAsync(
            take: _opt.BatchSize,
            lease: _opt.LeaseDuration,
            utcNow: DateTime.UtcNow,
            leaseOwner: _leaseOwner,
            ct: ct);

        if (jobs.Count == 0)
            return;

        _log.LogDebug(
            "Server transport apply jobs acquired. Count={Count}, LeaseOwner={LeaseOwner}",
            jobs.Count,
            _leaseOwner);

        await mediator.Send(
            new RunServerTransportApplyJobRequest(jobs, _leaseOwner),
            ct);
    }
    
    private async Task ApplyStartupJitterAsync(CancellationToken ct)
    {
        if (_opt.StartupJitterMs <= 0)
            return;

        var jitter = Random.Shared.Next(0, _opt.StartupJitterMs);
        if (jitter == 0)
            return;

        await Task.Delay(TimeSpan.FromMilliseconds(jitter), ct);
    }
    
    private static async Task DelayOrStopAsync(TimeSpan delay, CancellationToken ct)
    {
        if (delay <= TimeSpan.Zero)
            return;

        await Task.Delay(delay, ct);
    }
}

