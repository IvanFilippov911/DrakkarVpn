using DrakkarVpn.Bot.Application.Features.Start;
using MediatR;
using Telegram.Bot;

namespace DrakkarVpn.Bot.Application.Start;

public class StartCommandHandler : IRequestHandler<StartCommand, Unit>
{
    private readonly ITelegramBotClient _bot;

    public StartCommandHandler(ITelegramBotClient bot)
    {
        _bot = bot;
    }

    public async Task<Unit> Handle(StartCommand request, CancellationToken ct)
    {
        await _bot.SendTextMessageAsync(
            chatId: request.Message.Chat.Id,
            text: "Привет! Это Drakkar VPN Bot 🚀",
            cancellationToken: ct
        );
        return Unit.Value;
    }
}