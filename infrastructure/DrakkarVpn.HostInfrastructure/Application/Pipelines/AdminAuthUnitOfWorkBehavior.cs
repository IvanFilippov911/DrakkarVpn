using DrakkarVpn.AdminAuth.Application.Abstractions;
using MediatR;

namespace DrakkarVpn.Execution.Application.Pipelines;

public sealed class AdminAuthUnitOfWorkBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IAdminAuthCommand<TResponse>
{
    private readonly IAdminAuthUnitOfWork _uow;

    public AdminAuthUnitOfWorkBehavior(IAdminAuthUnitOfWork uow)
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
