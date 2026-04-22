using MediatR;
using NetworkMonitoring.Application.Abstractions;

namespace DrakkarVpn.Execution.Application.Pipelines;

public sealed class NetworkMonitoringUnitOfWorkBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : INetworkMonitoringCommand<TResponse>
{
    private readonly INetworkMonitoringUnitOfWork _uow;

    public NetworkMonitoringUnitOfWorkBehavior(INetworkMonitoringUnitOfWork uow)
        => _uow = uow;

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

