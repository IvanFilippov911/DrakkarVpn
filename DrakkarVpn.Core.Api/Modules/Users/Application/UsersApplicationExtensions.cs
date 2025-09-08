using DrakkarVpn.Core.Api.Modules.Users.Application.Pipeline;
using FluentValidation;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Users.Application;

internal interface IUsersApplicationMarker {}
internal sealed class UsersApplicationMarker : IUsersApplicationMarker {}

public static class UsersApplicationExtensions
{
    public static IServiceCollection AddUsersApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssemblyContaining<UsersApplicationMarker>());
        
        services.AddValidatorsFromAssemblyContaining<UsersApplicationMarker>();
        
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(EfTransactionBehavior<,>));
        
        return services;
    }
}