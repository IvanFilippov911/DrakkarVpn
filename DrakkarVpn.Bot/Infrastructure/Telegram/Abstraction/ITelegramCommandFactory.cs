using Telegram.Bot.Types;

namespace DrakkarVpn.Bot.Infrastructure.Telegram.Options;

public interface ITelegramCommandFactory
{
    string Name { get; }
    ITelegramCommand Create(Message message);
}
