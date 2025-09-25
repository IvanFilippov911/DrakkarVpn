using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;

namespace DrakkarVpn.Bot.Infrastructure.Telegram;

public class BotWorker : BackgroundService
{
    private readonly ITelegramBotClient _bot;
    private readonly ILogger<BotWorker> _logger;
    private readonly UpdateHandler _updateHandler;

    public BotWorker(
        ITelegramBotClient bot,
        ILogger<BotWorker> logger,
        UpdateHandler updateHandler)
    {
        _bot = bot;
        _logger = logger;
        _updateHandler = updateHandler;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var me = await _bot.GetMeAsync(stoppingToken);
        _logger.LogInformation("Bot started: {Username}", me.Username);

        _bot.StartReceiving(
            _updateHandler.HandleUpdateAsync,
            _updateHandler.HandleErrorAsync,
            new ReceiverOptions { AllowedUpdates = Array.Empty<UpdateType>() },
            stoppingToken
        );
    }
}