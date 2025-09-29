using DrakkarVpn.Bot.Application.Abstraction;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace DrakkarVpn.Bot.Infrastructure.Telegram;

public class UpdateHandler
{
    private readonly IEnumerable<IUpdateHandlerStrategy> _strategies;

    public UpdateHandler(IEnumerable<IUpdateHandlerStrategy> strategies)
    {
        _strategies = strategies;
    }

    public async Task HandleUpdateAsync(ITelegramBotClient bot, Update update, CancellationToken ct)
    {
        foreach (var strategy in _strategies)
        {
            if (strategy.CanHandle(update))
            {
                await strategy.HandleAsync(bot, update, ct);
                break;
            }
        }
    }

    public Task HandleErrorAsync(ITelegramBotClient bot, Exception ex, CancellationToken ct)
    {
        return Task.CompletedTask;
    }
}