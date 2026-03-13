using DrakkarVpn.Core.Api.Modules.Orchestrator.Application.DTOs;

namespace DrakkarVpn.Core.Api.Modules.Peers.API.Contracts.Response;

public sealed record GetVpnConfigResponse(
    VpnConfigStatus Status,
    string? ConfigRaw = null,
    string? HappLink  = null,
    Guid? JobId       = null,
    string? PollUrl   = null
)
{
    public static GetVpnConfigResponse Ready(string configRaw, string happLink)
        => new(VpnConfigStatus.Ready, ConfigRaw: configRaw, HappLink: happLink);

    public static GetVpnConfigResponse Pending(Guid jobId, string pollUrl)
        => new(VpnConfigStatus.Pending, JobId: jobId, PollUrl: pollUrl);
}