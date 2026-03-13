using DrakkarVpn.Core.Api.Modules.Users.Application.Abstractions;
using MediatR;

namespace DrakkarVpn.Execution.Application.Pipelines;

public sealed class UsersUnitOfWorkBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IUsersCommand<TResponse>
{
    private readonly IUsersUnitOfWork _uow;

    public UsersUnitOfWorkBehavior(IUsersUnitOfWork uow)
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