using DrakkarVpn.Core.Api.Modules.Users.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Users.Infrastructure;
using DrakkarVpn.Core.Api.Modules.Users.Infrastructure.Auth;
using DrakkarVpn.Core.Api.Modules.Users.Infrastructure.Repositories;
using DrakkarVpn.Core.Api.Modules.Users.Infrastructure.Security;
using DrakkarVpn.Core.Api.Modules.Users.Infrastructure.Telegram;
using DrakkarVpn.Users.Application.Abstractions;
using DrakkarVpn.Users.Application.Abstractions.Repositories;
using DrakkarVpn.Users.Application.Abstractions.Servers;
using DrakkarVpn.Users.Application.Abstractions.Services;
using DrakkarVpn.Users.Application.DTOs.Devices;
using DrakkarVpn.Users.Application.Features.Services;
using DrakkarVpn.Users.Application.Services;
using DrakkarVpn.Users.Application.Services.Devices;
using DrakkarVpn.Users.Infrastructure.Devices;
using DrakkarVpn.Users.Infrastructure.EF;
using DrakkarVpn.Users.Infrastructure.EF.Repositories.ReadRepositories;
using DrakkarVpn.Users.Infrastructure.Extensions;
using DrakkarVpn.Users.Infrastructure.Repositories;
using DrakkarVpn.Users.Infrastructure.Repositories.ReadRepositories;
using DrakkarVpn.Users.Infrastructure.Telegram;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DrakkarVpn.Users;

public static class Entry
{
    public static IServiceCollection AddUsersAdminHost(this IServiceCollection services)
    {
        services.AddScoped<IUserModerationService, UserModerationService>();
        services.AddScoped<IDevicesRevocationService, DevicesRevocationService>();
        services.AddScoped<IUsersQueryService, UsersQueryService>();
        
        return services;
    }
    
    public static IServiceCollection AddUsersUserHost(this IServiceCollection services,  IConfiguration configuration)
    {
        services.AddScoped<IUserRegistrationService, UserRegistrationService>();
        services.AddScoped<IConnectDeviceService, ConnectDeviceService>();
        services.AddScoped<IUsersQueryService, UsersQueryService>();
        services.AddScoped<ITelegramSessionValidationService, TelegramSessionValidationService>();
        services.AddScoped<IDeviceSessionService, DeviceSessionService>();
        services.AddScoped<IDeviceLifecycleService, DeviceLifecycleService>();
        
        services.AddScoped<IDeviceIdGenerator, DeviceIdGenerator>();
        if (configuration.GetValue("Auth:BypassTelegramInitData", false))
            services.AddScoped<ITelegramInitDataValidator, DevBypassTelegramValidator>();
        else
            services.AddScoped<ITelegramInitDataValidator, TelegramInitDataValidator>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddSingleton<IReplayStore, InMemoryReplayStore>();
        services.AddJwtAuth(configuration);
        services.AddTelegramOptions(configuration);

        return services;
    }
    
    public static IServiceCollection AddUsersInfrastructure(this IServiceCollection services, IConfiguration cfg)
    {
        var cs = cfg.GetConnectionString("Db")
                 ?? throw new InvalidOperationException("ConnectionString 'Db' not found");

        services.AddDbContext<UsersDbContext>(o => o.UseNpgsql(cs));
        
        services.AddScoped<IAppUserRepository, AppUserRepository>();
        services.AddScoped<IDeviceRepository, DeviceRepository>();
        services.AddScoped<IUserRealtimeReadStore,  UserRealtimeReadStore>();
        services.AddScoped<IAppUserReadStore, AppUserReadStore>();

        return services;
    }
}