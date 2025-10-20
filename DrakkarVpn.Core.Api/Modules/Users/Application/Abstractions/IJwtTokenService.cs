namespace DrakkarVpn.Core.Api.Modules.Users.Application.Abstractions;

public interface IJwtTokenService
{
    string IssueToken(
        long telegramId,
        string deviceId,
        TimeSpan? ttl = null,
        string? platform = null,
        string? deviceName = null);
}