using System.Net.Http.Json;
using DrakkarVpn.Bot.Infrastructure.Telegram.Abstraction;
using MediatR;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace DrakkarVpn.Bot.Application.Features.Start;

public class StartCommandHandler : IRequestHandler<StartCommand, Unit>
{
    private readonly ITelegramBotClient _bot;
    private readonly IOrchestratorClient _orchestrator;
    public StartCommandHandler(ITelegramBotClient bot, IOrchestratorClient orchestrator)
    {
        _bot = bot;
        _orchestrator = orchestrator;
    }

    public async Task<Unit> Handle(StartCommand req, CancellationToken ct)
    {
        var chatId = req.Message.Chat.Id;
        var firstName = req.Message.From?.FirstName ?? "друг";
        var tgId = req.Message.From?.Id ?? 0;

        await _orchestrator.RegisterUserAsync(tgId, ct);
        
        var webAppUrl = "https://info-construct-utility-colours.trycloudflare.com";

        var webApp = new WebAppInfo { Url = webAppUrl };
        var keyboard = new InlineKeyboardMarkup(
            InlineKeyboardButton.WithWebApp("🚀 Открыть VPN WebApp", webApp)
        );

        await _bot.SendTextMessageAsync(
            chatId: chatId,
            text: $"Привет, {firstName}! 🚀\n" +
                  $"Добро пожаловать на Драккар!\n" +
                  $"Нажми кнопку ниже, чтобы открыть приложение:",
            replyMarkup: keyboard,
            cancellationToken: ct);

        return Unit.Value;
    }
}

