namespace DrakkarVpn.Core.Api.Modules.Orchestrator.Application.DTOs;

public sealed record StatusVpnConfigDto(
    VpnConfigStatusDto Status,
    string? ConfigRaw = null,
    string? HappLink = null,
    Guid? JobId = null,
    string? PollUrl = null)
{
    public static StatusVpnConfigDto Ready(string configRaw, string happLink) =>
        new(VpnConfigStatusDto.Ready, ConfigRaw: configRaw, HappLink: happLink);

    public static StatusVpnConfigDto Pending(Guid jobId, string pollUrl) =>
        new(VpnConfigStatusDto.Pending, JobId: jobId, PollUrl: pollUrl);

    public static StatusVpnConfigDto NotStarted() =>
        new(VpnConfigStatusDto.NotStarted);
}
