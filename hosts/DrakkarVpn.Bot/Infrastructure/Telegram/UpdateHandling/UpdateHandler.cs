using DrakkarVpn.Bot.Application.Commands.Start;
using MediatR;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace DrakkarVpn.Bot.Infrastructure.Telegram.UpdateHandling;

public sealed class UpdateHandler
{
    private readonly IMediator _mediator;
    private readonly ILogger<UpdateHandler> _logger;

    public UpdateHandler(IMediator mediator, ILogger<UpdateHandler> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public async Task HandleUpdateAsync(ITelegramBotClient bot, Update update, CancellationToken ct)
    {
        var message = update.Message;
        if (message?.Text is null)
            return;

        var text = message.Text.Trim();

        if (text.StartsWith("/start", StringComparison.OrdinalIgnoreCase))
        {
            await _mediator.Send(new StartCommand(message), ct);
            return;
        }
        
        _logger.LogDebug("Unsupported message received: {Text}", text);
    }

    public Task HandleErrorAsync(ITelegramBotClient bot, Exception ex, CancellationToken ct)
    {
        _logger.LogError(ex, "Telegram polling error");
        return Task.CompletedTask;
    }
}