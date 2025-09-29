using DrakkarVpn.Bot.Infrastructure.Telegram.Abstraction;
using MediatR;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace DrakkarVpn.Bot.Application.Features.MyKeys;

public class MyKeysCommandHandler : IRequestHandler<MyKeysCommand, Unit>
{
    private readonly ITelegramBotClient _bot;
    private readonly IOrchestratorClient _orchestrator;
    private const int MaxPeersPerUser = 2;

    public MyKeysCommandHandler(
        ITelegramBotClient bot,
        IOrchestratorClient orchestrator)
    {
        _bot = bot;
        _orchestrator = orchestrator;
    }

    public async Task<Unit> Handle(MyKeysCommand req, CancellationToken ct)
    {
        var chatId = req.Message.Chat.Id;
        var tgId = req.Message.From!.Id;

        var peers = await _orchestrator.GetPeersAsync(tgId, ct);

        if (peers.Count == 0)
        {
            await _bot.SendTextMessageAsync(
                chatId,
                "У тебя пока нет активных ключей ⚓",
                cancellationToken: ct);
        }

        var buttons = peers
            .Select(p => new[]
            {
                InlineKeyboardButton.WithCallbackData(
                    $"🔑 {p.Id.ToString()[..6]}...",
                    $"peer:details:{p.Id}")
            })
            .ToList();

        if (peers.Count < MaxPeersPerUser)
        {
            buttons.Add(new[]
            {
                InlineKeyboardButton.WithCallbackData("➕ Добавить новый ключ", "peer:add")
            });
        }

        var keyboard = new InlineKeyboardMarkup(buttons);

        await _bot.SendTextMessageAsync(
            chatId,
            "Твои ключи:",
            replyMarkup: keyboard,
            cancellationToken: ct);

        return Unit.Value;
    }
}