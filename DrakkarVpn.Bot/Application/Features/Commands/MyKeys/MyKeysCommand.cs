using DrakkarVpn.Bot.Infrastructure.Telegram;
using DrakkarVpn.Bot.Infrastructure.Telegram.Options;
using Telegram.Bot.Types;

namespace DrakkarVpn.Bot.Application.Features.MyKeys;

public record MyKeysCommand(Message Message) : ITelegramCommand
{
    public string Name => "/mykeys";
}

public class MyKeysCommandFactory : ITelegramCommandFactory
{
    public string Name => "/mykeys";
    public ITelegramCommand Create(Message message) => new MyKeysCommand(message);
}