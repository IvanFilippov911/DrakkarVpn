using DrakkarVpn.Core.Api.Modules.Users.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Users.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DrakkarVpn.Core.Api.Modules.Users.Infrastructure;

public static class UsersInfrastructureExtensions
{
    public static IServiceCollection AddUsersInfrastructure(
        this IServiceCollection services,
        string? connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new InvalidOperationException("Connection string 'Default' is not configured.");
        
        services.AddDbContext<AppDbContext>(opt =>
        {
            opt.UseNpgsql(connectionString);
        });
        
        services.AddScoped<IAppUserRepository, AppUserRepository>();

        return services;
    }
}