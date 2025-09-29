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

    public StartCommandHandler(
        ITelegramBotClient bot,
        IOrchestratorClient orchestrator)
    {
        _bot = bot;
        _orchestrator = orchestrator;
        
    }

    public async Task<Unit> Handle(StartCommand req, CancellationToken ct)
    {
        var chatId = req.Message.Chat.Id;
        var tgId = req.Message.From!.Id;
        
        var result = await _orchestrator.RegisterUserAsync(tgId, ct);
        
        if (!result.IsNewUser)
        {
            await _bot.SendTextMessageAsync(
                chatId: chatId,
                text: "⚡ Ты уже с нами на борту!\n" +
                      "Используй /mykeys чтобы посмотреть свои ключи 🔑",
                cancellationToken: ct);

            return Unit.Value;
        }
        
        var regions = await _orchestrator.GetRegionsAsync(ct);
        
        var keyboard = new InlineKeyboardMarkup(
            regions.Select(r =>
                    InlineKeyboardButton.WithCallbackData(r.Code.ToUpper(), $"region:{r.Code}"))
                .Chunk(2)
        );
        
        await _bot.SendTextMessageAsync(
            chatId: chatId,
            text: $"Привет, {req.Message.From.FirstName}! 🚀\n" +
                  $"Вступай на наш Драккар! Здесь ты в безопасности.\n" +
                  $"Выбери, куда хочешь отправиться:",
            replyMarkup: keyboard,
            cancellationToken: ct);

        return Unit.Value;
    }
    
    
}

