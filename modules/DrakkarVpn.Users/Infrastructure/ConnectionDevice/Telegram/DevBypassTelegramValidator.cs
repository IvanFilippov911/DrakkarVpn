using DrakkarVpn.Core.Api.Modules.Users.Application.Abstractions;
using Microsoft.Extensions.Configuration;

namespace DrakkarVpn.Users.Infrastructure.Telegram;

/// <summary>
/// Dev-only initData validation when <c>Auth:BypassTelegramInitData</c> is true.
/// Uses a unique QueryId per call so replay protection does not block page reloads.
/// </summary>
public sealed class DevBypassTelegramValidator : ITelegramInitDataValidator
{
    private readonly bool _enabled;
    private readonly long _telegramId;

    public DevBypassTelegramValidator(IConfiguration cfg)
    {
        _enabled = cfg.GetValue("Auth:BypassTelegramInitData", false);
        _telegramId = cfg.GetValue("Auth:DevTelegramId", 7);
    }

    public (long TelegramId, long AuthDateUnix, string? QueryId) ValidateAndExtractAll(string initData)
    {
        if (!_enabled) throw new InvalidOperationException("Bypass disabled");

        var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        return (TelegramId: _telegramId, AuthDateUnix: now, QueryId: $"dev-{Guid.NewGuid():N}");
    }
}
