using DrakkarVpn.Core.Api.Modules.Admin.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Admin.Application.DTOs;
using DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Queries.GetUsersOverview;
using MediatR;

namespace DrakkarVpn.Admin.Api.Application.Features.Queries.Overview.GetUsersOverview;

public sealed class GetUsersOverviewHandler
    : IRequestHandler<GetUsersOverviewQuery, UsersOverviewDto>
{
    private readonly IAdminOverviewReadStore _store;

    public GetUsersOverviewHandler(IAdminOverviewReadStore store)
    {
        _store = store;
    }

    public Task<UsersOverviewDto> Handle(GetUsersOverviewQuery q, CancellationToken ct)
        => _store.GetUsersOverviewAsync(ct);
}