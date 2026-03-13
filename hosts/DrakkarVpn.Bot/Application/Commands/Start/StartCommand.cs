using MediatR;
using Telegram.Bot.Types;

namespace DrakkarVpn.Bot.Application.Commands.Start;

public record StartCommand(Message Message) : IRequest<Unit>
{
    public string Name => "/start";
}

