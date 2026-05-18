using DrakkarVpn.Servers.Application.Abstractions;
using MediatR;

namespace DrakkarVpn.Execution.Application.Pipelines;

public sealed class ServersUnitOfWorkBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IServersCommand<TResponse>
{
    private readonly IServersUnitOfWork _uow;

    public ServersUnitOfWorkBehavior(IServersUnitOfWork uow)
    {
        _uow = uow;
    }

    public Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken ct)
    {
        return _uow.ExecuteInTransactionAsync(async innerCt =>
        {
            var response = await next();
            await _uow.SaveChangesAsync(innerCt);
            return response;
        }, ct);
    }
}
