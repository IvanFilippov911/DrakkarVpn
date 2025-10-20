using DrakkarVpn.Core.Api.Modules.Subscriptions.Application.Features.Commands.ExpireSubscription;
using DrakkarVpn.Core.Api.Modules.Subscriptions.Application.Features.Queries.GetExpiredSubscriptions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace DrakkarVpn.Core.Api.Modules.Orchestrator.Infrastructure.BackgroundWorkers;

public sealed class SubscriptionExpirationWorker : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<SubscriptionExpirationWorker> _logger;

    public SubscriptionExpirationWorker(IServiceScopeFactory scopeFactory, ILogger<SubscriptionExpirationWorker> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();    
                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

                var expired = await mediator.Send(new GetExpiredSubscriptionsRequest(), stoppingToken);
                foreach (var sub in expired)
                    await mediator.Send(new ExpireSubscriptionRequest(sub.Id), stoppingToken);

                if (expired.Count > 0)
                    _logger.LogInformation("Expired {Count} subscriptions", expired.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SubscriptionExpirationWorker error");
            }

            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }

}