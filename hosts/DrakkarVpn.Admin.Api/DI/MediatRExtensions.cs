using DrakkarVpn.Core.Api.Modules.Servers.Application.Features.Commands.CleanupServerMetricsHistory;
using DrakkarVpn.Servers.Application.Handlers.RunServerTransportApplyJob;
using DrakkarVpn.Core.Api.Modules.Servers.Application.Handlers.RunServersPolling;
using MediatR;

namespace DrakkarVpn.Admin.Api.DI;

public static class MediatRExtensions
{
    public static IServiceCollection AddDrakkarMediatR(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
        });

        services.AddTransient<IRequestHandler<RunServersPollingCommand, Unit>, RunServersPollingHandler>();
        services.AddTransient<IRequestHandler<CleanupServerMetricsHistoryRequest, Unit>, CleanupServerMetricsHistoryHandler>();
        services.AddTransient<IRequestHandler<RunServerTransportApplyJobRequest, Unit>, RunServerTransportApplyJobHandler>();

        return services;
    }
}