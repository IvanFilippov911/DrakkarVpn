using DrakkarVpn.Bot.Infrastructure.Telegram.Abstraction;
using MediatR;
using Telegram.Bot;

namespace DrakkarVpn.Bot.Application.Features.RegionCallback;

public class RegionCallbackHandler : IRequestHandler<RegionCallbackCommand, Unit>
{
    private readonly ITelegramBotClient _bot;
    private readonly IOrchestratorClient _orchestrator;

    public RegionCallbackHandler(
        ITelegramBotClient bot,
        IOrchestratorClient orchestrator)
    {
        _bot = bot;
        _orchestrator = orchestrator;
    }

    public async Task<Unit> Handle(RegionCallbackCommand req, CancellationToken ct)
    {
        var chatId = req.CallbackQuery.Message!.Chat.Id;
        var tgId = req.CallbackQuery.From.Id;
        
        var regionCode = req.CallbackQuery.Data!.Split(':')[1];
        
        var peer = await _orchestrator.AllocatePeerAsync(tgId, regionCode, ct);
        
        await _bot.SendTextMessageAsync(
            chatId: chatId,
            text: $"✅ Твой ключ для региона {regionCode.ToUpper()}:\n```\n{peer.ConfigRaw}\n```",
            parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
            cancellationToken: ct);

        return Unit.Value;
    }
    
}
