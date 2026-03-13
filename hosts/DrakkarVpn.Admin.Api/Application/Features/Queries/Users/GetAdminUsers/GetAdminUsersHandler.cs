using DrakkarVpn.Core.Api.Modules.Admin.Application.DTOs;
using DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Queries.GetAdminServerUsers;
using DrakkarVpn.Shared;
using DrakkarVpn.Users.Application.Abstractions;
using DrakkarVpn.Users.Application.Features.Services;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Queries.GetAdminUsers;

public sealed class GetAdminUsersHandler
    : IRequestHandler<GetAdminUsersQuery, PagedResponseDto<AdminUserCardDto>>
{
    private readonly IUsersQueryService _usersQueryService;

    public GetAdminUsersHandler(IUsersQueryService usersQueryService)
    {
        _usersQueryService = usersQueryService;
    }

    public async Task<PagedResponseDto<AdminUserCardDto>> Handle(
        GetAdminUsersQuery q,
        CancellationToken ct)
    {
        var users = await _usersQueryService.GetUsersList(
            q.Page,  
            q.PageSize, 
            q.Search,
            q.Status,
            q.SubscriptionStatus,
            q.SortBy,
            q.UserSortDirection,
            ct);
        
        if (users.Items.Count == 0)
            return PagedResponseDto<AdminUserCardDto>.Empty(q.Page, q.PageSize);
        
        var items = users.Items
            .Select(u => new AdminUserCardDto(
                User:                u.User,
                Subscription:        u.Subscription,
                TrafficLast24hBytes: u.TrafficLast24hBytes
            ))
            .ToList();

        return PagedResponseDto<AdminUserCardDto>.From(
            items,
            users.Page,
            users.PageSize,
            users.Total);
    }
}