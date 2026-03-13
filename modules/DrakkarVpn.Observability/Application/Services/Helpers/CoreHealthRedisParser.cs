using System.Globalization;

namespace DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Queries.GetCoreHealth;

internal static class CoreHealthRedisParser
{
    public static double GetDouble(
        IReadOnlyDictionary<string, string> map,
        string key)
    {
        var s = map.GetValueOrDefault(key);
        return double.TryParse(
            s,
            NumberStyles.Float,
            CultureInfo.InvariantCulture,
            out var v)
            ? v
            : 0;
    }

    public static long GetLong(
        IReadOnlyDictionary<string, string> map,
        string key)
    {
        var s = map.GetValueOrDefault(key);
        return long.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out var v)
            ? v
            : 0L;
    }

    public static DateTime GetDate(
        IReadOnlyDictionary<string, string> map,
        string key)
    {
        var s = map.GetValueOrDefault(key);
        return DateTime.TryParse(
            s,
            CultureInfo.InvariantCulture,
            DateTimeStyles.RoundtripKind,
            out var dt)
            ? dt
            : DateTime.UtcNow;
    }
}