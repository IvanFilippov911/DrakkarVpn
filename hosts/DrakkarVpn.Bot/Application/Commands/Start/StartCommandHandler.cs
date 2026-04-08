using DrakkarVpn.Bot.Application.Abstractions;
using MediatR;
using Microsoft.Extensions.Configuration;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace DrakkarVpn.Bot.Application.Commands.Start;

public class StartCommandHandler : IRequestHandler<StartCommand, Unit>
{
    private readonly ITelegramBotClient _bot;
    private readonly IUserFlowClient _userClient;
    private readonly IConfiguration _configuration;

    public StartCommandHandler(
        ITelegramBotClient bot,
        IUserFlowClient userClient,
        IConfiguration configuration)
    {
        _bot = bot;
        _userClient = userClient;
        _configuration = configuration;
    }

    public async Task<Unit> Handle(StartCommand req, CancellationToken ct)
    {
        var chatId = req.Message.Chat.Id;
        var tgId = req.Message.From?.Id ?? 0;

        await _userClient.RegisterAsync(tgId, ct);
        await _bot.SendMessage(
            chatId: chatId,
            text: $"👇 Приватный доступ к сети",
            cancellationToken: ct);

        return Unit.Value;
    }
}

