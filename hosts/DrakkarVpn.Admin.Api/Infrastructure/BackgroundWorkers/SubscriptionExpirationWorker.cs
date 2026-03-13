using DrakkarVpn.Core.Api.Modules.Subscriptions.Application.Features.Commands.ExpireSubscription;
using DrakkarVpn.Core.Api.Modules.Subscriptions.Application.Features.Queries.GetExpiredSubscriptions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

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

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();    
                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

                var expired = await mediator.Send(new GetExpiredSubscriptionsRequest(), ct);
                if (expired.Count > 0)
                {
                    _logger.LogInformation("Expired {Count} subscriptions", expired.Count);
                    await mediator.Send(
                        new ExpireSubscriptionsBulkCommand(expired), ct);
                }
                
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SubscriptionExpirationWorker error");
            }

            await Task.Delay(TimeSpan.FromMinutes(1), ct);
        }
    }

}