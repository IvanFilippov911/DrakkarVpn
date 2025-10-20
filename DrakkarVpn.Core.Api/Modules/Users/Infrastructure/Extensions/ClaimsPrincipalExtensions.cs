using System.Security.Claims;

namespace DrakkarVpn.Core.Api.Modules.Users.Infrastructure;

public static class ClaimsPrincipalExtensions
{
    public static long GetTelegramId(this ClaimsPrincipal user)
        => long.TryParse(user.FindFirstValue("telegram_id"), out var id)
            ? id : throw new UnauthorizedAccessException("telegram_id claim missing");

    public static string GetDeviceId(this ClaimsPrincipal user)
        => user.FindFirstValue("device_id")
           ?? throw new UnauthorizedAccessException("device_id claim missing");

    public static string? GetPlatform(this ClaimsPrincipal user)
        => user.FindFirstValue("platform");

    public static string? GetDeviceName(this ClaimsPrincipal user)
        => user.FindFirstValue("device_name");
}