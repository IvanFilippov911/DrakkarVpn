using DrakkarVpn.Bot.Infrastructure.Telegram.Abstraction;
using MediatR;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace DrakkarVpn.Bot.Infrastructure.Telegram;

public class UpdateHandler
{
    private readonly IMediator _mediator;
    private readonly ICommandRegistry _registry;

    public UpdateHandler(IMediator mediator, ICommandRegistry registry)
    {
        _mediator = mediator;
        _registry = registry;
    }

    public async Task HandleUpdateAsync(ITelegramBotClient bot, Update update, CancellationToken ct)
    {
        if (update.Message is not { } message)
            return;

        var command = _registry.ResolveCommand(message);
        if (command is null)
            return;

        await _mediator.Send(command, ct);
    }

    public Task HandleErrorAsync(ITelegramBotClient bot, Exception ex, CancellationToken ct)
    {
        return Task.CompletedTask;
    }
}