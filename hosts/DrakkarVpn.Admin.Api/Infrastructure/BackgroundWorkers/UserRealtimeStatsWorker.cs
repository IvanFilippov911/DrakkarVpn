using DrakkarVpn.Admin.Api.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Admin.Infrastructure.EF;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DrakkarVpn.Users.Infrastructure.BackgroundWorker;

public sealed class UserRealtimeStatsWorker : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<UserRealtimeStatsWorker> _logger;

    public UserRealtimeStatsWorker(
        IServiceScopeFactory scopeFactory,
        ILogger<UserRealtimeStatsWorker> logger)
    {
        _scopeFactory = scopeFactory;
        _logger       = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var updater = scope.ServiceProvider.GetRequiredService<IUserRealtimeStatsUpdater>();
                var db = scope.ServiceProvider.GetRequiredService<AdminReadDbContext>();

                await updater.UpdateAllUsersAsync(stoppingToken);
                await db.SaveChangesAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update user realtime stats");
            }

            await Task.Delay(TimeSpan.FromSeconds(40), stoppingToken);
        }
    }
}
