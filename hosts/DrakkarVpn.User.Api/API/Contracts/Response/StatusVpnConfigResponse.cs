using DrakkarVpn.Core.Api.Modules.Orchestrator.Application.DTOs;

namespace DrakkarVpn.Core.Api.Modules.Peers.API.Contracts.Response;

public sealed record StatusVpnConfigResponse(
    VpnConfigStatusDto StatusDto,
    string? ConfigRaw = null,
    string? ConfigVless = null,
    string? ConfigUrl = null,
    string? HappLink  = null,
    string? V2rayLink = null,
    Guid? JobId       = null,
    string? PollUrl   = null
)
{
    public static StatusVpnConfigResponse Ready(
        string configRaw,
        string configVless,
        string configUrl,
        string happLink,
        string v2rayLink)
        => new(
            VpnConfigStatusDto.Ready,
            ConfigRaw: configRaw,
            ConfigVless: configVless,
            ConfigUrl: configUrl,
            HappLink: happLink,
            V2rayLink: v2rayLink);

    public static StatusVpnConfigResponse Pending(Guid jobId, string pollUrl)
        => new(VpnConfigStatusDto.Pending, JobId: jobId, PollUrl: pollUrl);

    public static StatusVpnConfigResponse NotStarted()
        => new(VpnConfigStatusDto.NotStarted);
}