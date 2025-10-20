using DrakkarVpn.Shared.Options;

namespace DrakkarVpn.Core.Api.Modules.Users.Infrastructure;

public static class TelegramOptionsExtensions
{
    public static IServiceCollection AddTelegramOptions(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddOptions<TelegramOptions>()
            .Bind(configuration.GetSection("Telegram"))
            .Validate(o => !string.IsNullOrWhiteSpace(o.BotToken),
                "Telegram:BotToken is missing or empty")
            .ValidateOnStart();

        return services;
    }
}