namespace DrakkarVpn.Core.Api.Modules.Admin.Application.DTOs;

public sealed record ServersOverviewDto(
    int      TotalServers,
    int      ServersOnline,
    int      TotalActivePeers,
    int      PeersOnline
);