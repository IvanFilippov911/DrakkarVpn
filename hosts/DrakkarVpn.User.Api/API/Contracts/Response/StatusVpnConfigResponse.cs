using DrakkarVpn.Core.Api.Modules.Orchestrator.Application.DTOs;

namespace DrakkarVpn.Core.Api.Modules.Peers.API.Contracts.Response;

public sealed record StatusVpnConfigResponse(
    VpnConfigStatusDto StatusDto,
    string? ConfigRaw = null,
    string? HappLink  = null,
    Guid? JobId       = null,
    string? PollUrl   = null
)
{
    public static StatusVpnConfigResponse Ready(string configRaw, string happLink)
        => new(VpnConfigStatusDto.Ready, ConfigRaw: configRaw, HappLink: happLink);

    public static StatusVpnConfigResponse Pending(Guid jobId, string pollUrl)
        => new(VpnConfigStatusDto.Pending, JobId: jobId, PollUrl: pollUrl);

    public static StatusVpnConfigResponse NotStarted()
        => new(VpnConfigStatusDto.NotStarted);
}