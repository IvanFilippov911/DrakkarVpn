using DrakkarVpn.Bot.Infrastructure.Telegram;
using DrakkarVpn.Bot.Infrastructure.Telegram.Options;
using Telegram.Bot.Types;
using MediatR;

namespace DrakkarVpn.Bot.Application.Features.Start;

public record StartCommand(Message Message) : ITelegramCommand
{
    public string Name => "/start";
}

public class StartCommandFactory : ITelegramCommandFactory
{
    public string Name => "/start";
    public ITelegramCommand Create(Message message) => new StartCommand(message);
}