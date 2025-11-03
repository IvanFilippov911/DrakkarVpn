using Xray.App.Stats.Command;

namespace DrakkarVpn.Agent.Application.Peers.Services;

public static class XrayUserStatsParser
{
    private const string UserMarker     = "user>>>";
    private const string UplinkSuffix   = ">>>traffic>>>uplink";
    private const string DownlinkSuffix = ">>>traffic>>>downlink";
    
    public static Dictionary<string, (long? up, long? down)> BuildTrafficByEmail(
        IReadOnlyList<Stat> stats)
    {
        var dict = new Dictionary<string, (long? up, long? down)>(
            StringComparer.OrdinalIgnoreCase);

        foreach (var s in stats)
        {
            var name    = s.Name;
            var userIdx = name.IndexOf(UserMarker, StringComparison.Ordinal);
            if (userIdx == -1)
                continue;

            var start = userIdx + UserMarker.Length;

            bool isUp;
            string suffix;

            if (name.EndsWith(UplinkSuffix, StringComparison.OrdinalIgnoreCase))
            {
                suffix = UplinkSuffix;
                isUp   = true;
            }
            else if (name.EndsWith(DownlinkSuffix, StringComparison.OrdinalIgnoreCase))
            {
                suffix = DownlinkSuffix;
                isUp   = false;
            }
            else
            {
                continue;
            }

            var emailEnd = name.Length - suffix.Length;
            if (emailEnd <= start)
                continue;

            var email = name.Substring(start, emailEnd - start);
            if (string.IsNullOrWhiteSpace(email))
                continue;

            if (!dict.TryGetValue(email, out var t))
                t = (null, null);

            if (isUp)
                t.up = s.Value;
            else
                t.down = s.Value;

            dict[email] = t;
        }

        return dict;
    }
}