namespace DrakkarVpn.Core.Api.Modules.Admin.Application.DTOs;

public sealed record ServersWithTrafficOverviewDto(
    ServersOverviewDto Servers,
    TrafficOverviewDto Traffic
);