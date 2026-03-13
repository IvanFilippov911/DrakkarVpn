using DrakkarVpn.Core.Api.Idempotency.Application.Abstracts;
using Idempotency.Idempotency.Infrastructure.Pipelines;
using Idempotency.Infrastructure.EF;
using Idempotency.Infrastructure.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DrakkarVpn.Idempotency;

public static class Entry
{
    public static IServiceCollection AddIdempotencyInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var cs = configuration.GetConnectionString("Db")
                 ?? throw new InvalidOperationException("ConnectionString 'Db' not found");

        services.AddDbContext<IdempotencyDbContext>(o =>
            o.UseNpgsql(cs));

        services.AddScoped<IIdempotencyRepository, IdempotencyRepository>();

        return services;
    }

    public static IServiceCollection AddIdempotencyPipeline(this IServiceCollection services)
    {
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(IdempotencyBehavior<,>));

        return services;
    }
}