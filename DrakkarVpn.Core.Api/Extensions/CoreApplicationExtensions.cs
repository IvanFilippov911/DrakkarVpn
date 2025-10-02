using System.Reflection;
using DrakkarVpn.Core.Pipeline;
using FluentValidation;
using MediatR;

namespace DrakkarVpn.Core.Api.Extensions;

public static class CoreApplicationExtensions
{
    public static IServiceCollection AddCoreApplication(
        this IServiceCollection services,
        params Assembly[] assemblies)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(assemblies));
        services.AddValidatorsFromAssemblies(assemblies);
        
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(IdempotencyBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(EfTransactionBehavior<,>));
        


        return services;
    }
}