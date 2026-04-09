namespace DrakkarVpn.Bot.Application.Abstractions;

public interface IUserFlowClient
{
    Task RegisterAsync(long telegramId, string? username, CancellationToken ct);
}