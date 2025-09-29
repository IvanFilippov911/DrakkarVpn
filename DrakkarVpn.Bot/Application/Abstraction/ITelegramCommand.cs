using MediatR;
using Telegram.Bot.Types;

namespace DrakkarVpn.Bot.Infrastructure.Telegram;

public interface ITelegramCommand : IRequest<Unit>
{
    Message Message { get; }
}
