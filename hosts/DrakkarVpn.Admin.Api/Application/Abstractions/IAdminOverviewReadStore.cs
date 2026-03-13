using DrakkarVpn.Core.Api.Modules.Admin.Application.DTOs;

namespace DrakkarVpn.Core.Api.Modules.Admin.Application.Abstractions;

public interface IAdminOverviewReadStore
{
    Task<ServersOverviewDto> GetServersOverviewAsync(CancellationToken ct);
    Task<UsersOverviewDto>   GetUsersOverviewAsync(CancellationToken ct);
    Task<TrafficOverviewDto> GetTrafficOverviewAsync(CancellationToken ct);
}