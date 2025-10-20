namespace DrakkarVpn.Core.Api.Modules.Orchestrator.Application.DTOs;

public sealed record GetVpnConfigResponse(
    string Config,
    string HappLink
);
