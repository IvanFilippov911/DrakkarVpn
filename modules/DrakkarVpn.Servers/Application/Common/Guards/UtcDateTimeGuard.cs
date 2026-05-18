using System.Runtime.CompilerServices;

namespace DrakkarVpn.Servers.Application.Common.Guards;

internal static class UtcDateTimeGuard
{
    public static DateTime RequireUtc(
        DateTime value,
        [CallerArgumentExpression(nameof(value))] string? paramName = null)
    {
        if (value.Kind != DateTimeKind.Utc)
            throw new ArgumentException("Value must be a UTC DateTime.", paramName);

        return value;
    }
}
