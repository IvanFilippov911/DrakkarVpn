using DrakkarVpn.AdminAuth.Application.Abstractions.Repositories;
using DrakkarVpn.AdminAuth.Application.Options;
using DrakkarVpn.AdminAuth.Infrastructure.EF;
using DrakkarVpn.AdminAuth.Infrastructure.EF.Repositories;
using DrakkarVpn.AdminAuth.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DrakkarVpn.AdminAuth.DI;

public static class AdminAuthInfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddAdminAuthInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        var connectionString = configuration.GetConnectionString("Db")
                               ?? throw new InvalidOperationException("ConnectionString 'Db' not found");
        var lockoutSection = configuration.GetSection(AdminLockoutOptions.SectionName);
        var lockoutOptions = lockoutSection.Get<AdminLockoutOptions>() ?? new AdminLockoutOptions();

        services.AddDbContext<AdminAuthDbContext>(options =>
            options.UseNpgsql(connectionString));

        services
            .AddIdentityCore<AdminIdentityUser>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.Lockout.AllowedForNewUsers = lockoutOptions.AllowedForNewUsers;
                options.Lockout.MaxFailedAccessAttempts = lockoutOptions.MaxFailedAccessAttempts;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(lockoutOptions.DefaultLockoutMinutes);
            })
            .AddRoles<AdminIdentityRole>()
            .AddEntityFrameworkStores<AdminAuthDbContext>();

        services.AddScoped<IAdminRefreshTokenRepository, AdminRefreshTokenRepository>();

        return services;
    }
}
