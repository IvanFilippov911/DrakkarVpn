using Telegram.Bot;
using Telegram.Bot.Types;

namespace DrakkarVpn.Bot.Application.Abstraction;

public interface IUpdateHandlerStrategy
{
    bool CanHandle(Update update);
    Task HandleAsync(ITelegramBotClient bot, Update update, CancellationToken ct);
}