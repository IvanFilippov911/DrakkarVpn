using DrakkarVpn.Bot.Infrastructure.Telegram;
using DrakkarVpn.Bot.Infrastructure.Telegram.Options;
using Telegram.Bot.Types;

namespace DrakkarVpn.Bot.Application.Features.Help;

public record HelpCommand(Message Message) : ITelegramCommand
{
    public string Name => "/help";
}

public class HelpCommandFactory : ITelegramCommandFactory
{
    public string Name => "/help";
    public ITelegramCommand Create(Message message) => new HelpCommand(message);
}