using Telegram.Bot.Types;

namespace DrakkarVpn.Bot.Infrastructure.Telegram.Abstraction;

public interface ICommandRegistry
{
    ITelegramCommand? ResolveCommand(Message message);
}