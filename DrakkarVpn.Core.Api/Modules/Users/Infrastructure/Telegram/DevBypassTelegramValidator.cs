using DrakkarVpn.Core.Api.Modules.Users.Application.Abstractions;

namespace DrakkarVpn.Core.Api.Modules.Users.Infrastructure.Telegram;

public sealed class DevBypassTelegramValidator : ITelegramInitDataValidator
{
    private readonly bool _enabled;
    public DevBypassTelegramValidator(IConfiguration cfg)
        => _enabled = cfg.GetValue("Auth:BypassTelegramInitData", false);

    public (long TelegramId, long AuthDateUnix, string? QueryId) ValidateAndExtractAll(string initData)
    {
        if (!_enabled) throw new InvalidOperationException("Bypass disabled");
        
        var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        return (TelegramId: 6, AuthDateUnix: now, QueryId: "dev-qid");
    }
}