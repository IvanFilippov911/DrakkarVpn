using DrakkarVpn.Bot.Application.Abstraction;
using DrakkarVpn.Bot.Application.Features.RegionCallback;
using MediatR;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace DrakkarVpn.Bot.Application.Features.Common;

public class CallbackUpdateStrategy : IUpdateHandlerStrategy
{
    private readonly IMediator _mediator;

    public CallbackUpdateStrategy(IMediator mediator)
    {
        _mediator = mediator;
    }

    public bool CanHandle(Update update) => update.CallbackQuery is not null;

    public async Task HandleAsync(ITelegramBotClient bot, Update update, CancellationToken ct)
    {
        var callback = update.CallbackQuery!;
        
        if (!string.IsNullOrEmpty(callback.Data) && callback.Data.StartsWith("region:"))
        {
            await _mediator.Send(new RegionCallbackCommand(callback), ct);
            await bot.AnswerCallbackQueryAsync(callback.Id, cancellationToken: ct);
        }
    }
}