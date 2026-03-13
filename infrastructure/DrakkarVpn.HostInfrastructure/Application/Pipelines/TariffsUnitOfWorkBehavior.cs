using DrakkarVpn.Core.Api.Modules.Tariffs.Application.Abstracts;
using MediatR;

namespace DrakkarVpn.Execution.Application.Pipelines;

public sealed class TariffsUnitOfWorkBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : ITariffsCommand<TResponse>
{
    private readonly ITariffsUnitOfWork _uow;

    public TariffsUnitOfWorkBehavior(ITariffsUnitOfWork uow)
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