using DrakkarVpn.Admin.Api.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Admin.Application.Abstractions;

namespace DrakkarVpn.Admin.Api.Infrastructure.BackgroundWorkers;

public sealed class ServerRealtimeStatsWorker : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<ServerRealtimeStatsWorker> _log;

    public ServerRealtimeStatsWorker(IServiceScopeFactory scopeFactory, ILogger<ServerRealtimeStatsWorker> log)
    {
        _scopeFactory = scopeFactory;
        _log = log;
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var orch = scope.ServiceProvider.GetRequiredService<IServerRealtimeStatsUpdater>();

                var nowUtc = DateTime.UtcNow;
                var updated = await orch.UpdateNowAsync(nowUtc, ct);

                _log.LogInformation("ServerRealtimeStats updated: {Count}", updated);
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "ServerRealtimeStatsWorker failed");
            }

            await Task.Delay(TimeSpan.FromMinutes(1), ct);
        }
    }
}