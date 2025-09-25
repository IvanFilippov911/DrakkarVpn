using DrakkarVpn.Bot.Infrastructure.Telegram.Abstraction;
using DrakkarVpn.Bot.Infrastructure.Telegram.Options;
using MediatR;
using Telegram.Bot.Types;

namespace DrakkarVpn.Bot.Infrastructure.Telegram;

public class CommandRegistry : ICommandRegistry
{
    private readonly Dictionary<string, ITelegramCommandFactory> _factories;

    public CommandRegistry(IEnumerable<ITelegramCommandFactory> factories)
    {
        _factories = factories.ToDictionary(f => f.Name, f => f);
    }

    public ITelegramCommand? ResolveCommand(Message message)
    {
        if (string.IsNullOrWhiteSpace(message.Text))
            return null;

        var cmdText = message.Text.Split(' ')[0];
        return _factories.TryGetValue(cmdText, out var factory)
            ? factory.Create(message)
            : null;
    }
}
