namespace DrakkarVpn.Core.Api.Modules.Orchestrator.Application.DTOs;

public sealed record StatusVpnConfigDto(
    VpnConfigStatusDto Status,
    string? ConfigRaw = null,
    string? ConfigVless = null,
    string? ConfigUrl = null,
    string? HappLink = null,
    string? V2rayLink = null,
    Guid? JobId = null,
    string? PollUrl = null)
{
    public static StatusVpnConfigDto Ready(
        string configRaw,
        string configVless,
        string configUrl,
        string happLink,
        string v2rayLink) =>
        new(
            VpnConfigStatusDto.Ready,
            ConfigRaw: configRaw,
            ConfigVless: configVless,
            ConfigUrl: configUrl,
            HappLink: happLink,
            V2rayLink: v2rayLink);

    public static StatusVpnConfigDto Pending(Guid jobId, string pollUrl) =>
        new(VpnConfigStatusDto.Pending, JobId: jobId, PollUrl: pollUrl);

    public static StatusVpnConfigDto NotStarted() =>
        new(VpnConfigStatusDto.NotStarted);
}
