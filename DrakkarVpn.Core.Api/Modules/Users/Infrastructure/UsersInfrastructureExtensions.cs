using DrakkarVpn.Core.Api.Infrastructure.EF;
using DrakkarVpn.Core.Api.Modules.Users.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Users.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DrakkarVpn.Core.Api.Modules.Users.Infrastructure;

public static class UsersInfrastructureExtensions
{
    public static IServiceCollection AddUsersInfrastructure(
        this IServiceCollection services)
    {
        
        services.AddScoped<IAppUserRepository, AppUserRepository>();

        return services;
    }
}