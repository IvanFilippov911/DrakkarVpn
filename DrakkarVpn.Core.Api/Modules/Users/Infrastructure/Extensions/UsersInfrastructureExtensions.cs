using DrakkarVpn.Core.Api.Infrastructure.EF;
using DrakkarVpn.Core.Api.Modules.Users.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Users.Infrastructure.Auth;
using DrakkarVpn.Core.Api.Modules.Users.Infrastructure.Devices;
using DrakkarVpn.Core.Api.Modules.Users.Infrastructure.Repositories;
using DrakkarVpn.Core.Api.Modules.Users.Infrastructure.Security;
using DrakkarVpn.Core.Api.Modules.Users.Infrastructure.Telegram;
using Microsoft.EntityFrameworkCore;

namespace DrakkarVpn.Core.Api.Modules.Users.Infrastructure;

public static class UsersInfrastructureExtensions
{
    public static IServiceCollection AddUsersInfrastructure(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMemoryCache();

        services.AddTelegramOptions(configuration);
        //services.AddJwtAuth(configuration);
        
        services.AddScoped<IAppUserRepository, AppUserRepository>();
        services.AddScoped<IDeviceRepository, DeviceRepository>();

        services.AddSingleton<IJwtTokenService, JwtTokenService>();
        
        if (configuration.GetValue("Auth:BypassTelegramInitData", false))
            services.AddSingleton<ITelegramInitDataValidator, DevBypassTelegramValidator>();
        else
            services.AddSingleton<ITelegramInitDataValidator, TelegramInitDataValidator>();
        services.AddSingleton<IReplayStore, InMemoryReplayStore>();
        services.AddSingleton<IDeviceIdGenerator, DeviceIdGenerator>();
        
        services.AddJwtAuth(configuration);

        return services;
    }
}