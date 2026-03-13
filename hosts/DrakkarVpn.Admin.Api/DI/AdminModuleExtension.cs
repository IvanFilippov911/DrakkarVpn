using DrakkarVpn.Admin.Api.Application.Abstractions;
using DrakkarVpn.Admin.Api.Application.Features.Services;
using DrakkarVpn.Admin.Api.Infrastructure.EF.Repository;
using DrakkarVpn.Admin.Api.Infrastructure.EF.Repository.ReadRepositories;
using DrakkarVpn.Core.Api.Modules.Admin.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Services;
using DrakkarVpn.Core.Api.Modules.Admin.Infrastructure.EF;
using DrakkarVpn.Users.Application.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace DrakkarVpn.Admin.Api.DI;

public static class AdminModuleExtension
{
    public static IServiceCollection AddAdminInfrastructureModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Db");

        services.AddDbContext<AdminReadDbContext>(opt =>
        {
            opt.UseNpgsql(connectionString);
            opt.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        });

        services.AddScoped<IAdminOverviewReadStore, AdminOverviewReadStore>();
        services.AddScoped<IServerRealtimeStatsUpdater, ServerRealtimeStatsUpdater>();
        services.AddScoped<IUserRealtimeStatsUpdater, UserRealtimeStatsUpdater>();
        services.AddScoped<IUserDevicesReadStore, UserDevicesReadStore>();

        return services;
    }
}