using MediatR;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace DrakkarVpn.Bot.Application.Commands.Start;

public class StartCommandHandler : IRequestHandler<StartCommand, Unit>
{
    private readonly ITelegramBotClient _bot;

    public StartCommandHandler(ITelegramBotClient bot) => _bot = bot;

    public async Task<Unit> Handle(StartCommand req, CancellationToken ct)
    {
        var chatId = req.Message.Chat.Id;

        await _bot.SendMessage(
            chatId: chatId,
            text: $"👇 Приватный доступ к сети",
            cancellationToken: ct);

        return Unit.Value;
    }
}
