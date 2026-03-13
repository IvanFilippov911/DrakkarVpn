namespace DrakkarVpn.Bot.Infrastructure.Telegram.Options;

public sealed class TelegramOptions
{
    public const string SectionName = "Telegram";

    public string Token { get; set; } = string.Empty;
}
