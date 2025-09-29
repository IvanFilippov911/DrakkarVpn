using DrakkarVpn.Core.Api.Modules.Subscriptions.Application.Features.Commands.ExpireSubscription;
using DrakkarVpn.Core.Api.Modules.Subscriptions.Application.Features.Queries.GetExpiredSubscriptions;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Orchestrator.Infrastructure.BackgroundWorkers;

public sealed class SubscriptionExpirationWorker : BackgroundService
{
    private readonly IMediator _mediator;
    private readonly ILogger<SubscriptionExpirationWorker> _logger;

    public SubscriptionExpirationWorker(IMediator mediator, ILogger<SubscriptionExpirationWorker> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("SubscriptionExpirationWorker started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var expired = await _mediator.Send(new GetExpiredSubscriptionsRequest(), stoppingToken);

                foreach (var sub in expired)
                {
                    await _mediator.Send(new ExpireSubscriptionRequest(sub.Id), stoppingToken);
                }

                if (expired.Count > 0)
                    _logger.LogInformation("Expired {Count} subscriptions", expired.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in SubscriptionExpirationWorker");
            }

            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }
}