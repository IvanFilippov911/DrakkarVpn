using DrakkarVpn.Core.Api.Modules.Subscriptions.Application.Abstractions;
using MediatR;

namespace DrakkarVpn.Execution.Application.Pipelines;

public sealed class SubscriptionsUnitOfWorkBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : ISubscriptionsCommand<TResponse>
{
    private readonly ISubscriptionsUnitOfWork _uow;

    public SubscriptionsUnitOfWorkBehavior(ISubscriptionsUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken ct)
    {
        var response = await next();
        await _uow.SaveChangesAsync(ct);
        return response;
    }
}