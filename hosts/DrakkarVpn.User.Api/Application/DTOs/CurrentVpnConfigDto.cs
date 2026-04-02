namespace DrakkarVpn.Core.Api.Modules.Orchestrator.Application.Abstractions;

public sealed record CurrentVpnConfigDto(
    string ConfigRaw,
    string ConfigVless,
    string ConfigUrl,
    string HappLink,
    string V2rayLink);

