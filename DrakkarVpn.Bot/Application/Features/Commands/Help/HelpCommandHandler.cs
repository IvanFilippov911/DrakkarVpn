using MediatR;
using Telegram.Bot;

namespace DrakkarVpn.Bot.Application.Features.Help;

public class HelpCommandHandler : IRequestHandler<HelpCommand,  Unit>
{
    private readonly ITelegramBotClient _bot;

    public HelpCommandHandler(ITelegramBotClient bot)
    {
        _bot = bot;
    }

    public async Task<Unit> Handle(HelpCommand request, CancellationToken ct)
    {
        await _bot.SendTextMessageAsync(
            chatId: request.Message.Chat.Id,
            text: "Доступные команды:\n/start - запуск\n/help - помощь",
            cancellationToken: ct
        );
        return Unit.Value;
    }
    
}