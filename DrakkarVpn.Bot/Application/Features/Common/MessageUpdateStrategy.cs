using DrakkarVpn.Bot.Application.Abstraction;
using DrakkarVpn.Bot.Infrastructure.Telegram.Abstraction;
using MediatR;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace DrakkarVpn.Bot.Application.Features.Common;

public class MessageUpdateStrategy : IUpdateHandlerStrategy
{
    private readonly IMediator _mediator;
    private readonly ICommandRegistry _registry;

    public MessageUpdateStrategy(IMediator mediator, ICommandRegistry registry)
    {
        _mediator = mediator;
        _registry = registry;
    }

    public bool CanHandle(Update update) => update.Message is not null;

    public async Task HandleAsync(ITelegramBotClient bot, Update update, CancellationToken ct)
    {
        var msg = update.Message!;
        var command = _registry.ResolveCommand(msg);
        if (command is not null)
            await _mediator.Send(command, ct);
    }
}