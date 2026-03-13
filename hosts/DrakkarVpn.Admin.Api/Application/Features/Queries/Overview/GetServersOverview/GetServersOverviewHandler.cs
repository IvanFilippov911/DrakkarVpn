using DrakkarVpn.Core.Api.Modules.Admin.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Admin.Application.DTOs;
using DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Queries.GetServersOverview;
using MediatR;

namespace DrakkarVpn.Admin.Api.Application.Features.Queries.Overview.GetServersOverview;

public sealed class GetServersOverviewHandler
    : IRequestHandler<GetServersOverviewQuery, ServersWithTrafficOverviewDto>
{
    private readonly IAdminOverviewReadStore _store;

    public GetServersOverviewHandler(IAdminOverviewReadStore store)
        => _store = store;

    public async Task<ServersWithTrafficOverviewDto> Handle(
        GetServersOverviewQuery q,
        CancellationToken ct)
    {
        var servers = await _store.GetServersOverviewAsync(ct);
        var traffic = await _store.GetTrafficOverviewAsync(ct);

        return new ServersWithTrafficOverviewDto(servers, traffic);
    }
}