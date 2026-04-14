using DrakkarVpn.Core.Api.Modules.Servers.Domain;
using DrakkarVpn.Servers.Domain.Inputs;

namespace DrakkarVpn.Servers.Domain.Policies;

public static class ServerStatusPolicy
{
    private const int DisableFailuresThreshold = 3;
    private const double DrainingPeersThreshold = 0.95;

    public static ServerStatus Compute(
        ServerStatus current,
        ServerPollResult result,
        int? maxPeers)
    {
        if (!result.Reachable && result.ConsecutiveFailures >= DisableFailuresThreshold)
            return ServerStatus.Disabled;

        if (!result.Reachable)
            return current;

        if (maxPeers is > 0)
        {
            var load = (double)result.PeersActive / maxPeers.Value;
            if (load >= DrainingPeersThreshold)
                return ServerStatus.Draining;
        }

        return ServerStatus.Enabled;
    }
}