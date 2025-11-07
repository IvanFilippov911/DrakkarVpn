using DrakkarVpn.Core.Api.Modules.Admin.Application.DTOs;
using DrakkarVpn.Core.Api.Modules.Users.Application.Features.Queries.GetListServerUsers;
using DrakkarVpn.Core.Api.Modules.Users.Application.Features.Queries.GetUsersTrafficSummary;
using DrakkarVpn.Shared;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Queries.GetAdminServerUsers;

public sealed class GetAdminServerUsersHandler
    : IRequestHandler<GetAdminServerUsersQuery, PagedResponseDto<AdminUserCardDto>>
{
    private readonly IMediator _mediator;

    public GetAdminServerUsersHandler(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<PagedResponseDto<AdminUserCardDto>> Handle(
        GetAdminServerUsersQuery q,
        CancellationToken ct)
    {
        var baseResult = await _mediator.Send(new UsersOnServerRequest(
            ServerId:           q.ServerId,
            Page:               q.Page,
            PageSize:           q.PageSize,
            Search:             q.Search,
            Status:             q.Status,
            SubscriptionStatus: q.SubscriptionStatus,
            SortBy:             q.SortBy
        ), ct);

        if (baseResult.Items.Count == 0)
            return PagedResponseDto<AdminUserCardDto>.Empty(q.Page, q.PageSize);

        var userIds = baseResult.Items
            .Select(i => i.User.Id)
            .Distinct()
            .ToArray();

        var nowUtc   = DateTime.UtcNow;
        var from24h  = nowUtc.AddDays(-1);
        
        var traffic = await _mediator.Send(
            new GetUsersTrafficSummaryRequest(
                ServerId:   q.ServerId,
                UserIds:    userIds,
                From24hUtc: from24h
            ),
            ct);
        
        var items = baseResult.Items.Select(card =>
        {
            traffic.TryGetValue(card.User.Id, out var t);

            return new AdminUserCardDto(
                User:                 card.User,
                Subscription:         card.Subscription,
                TrafficLast24hBytes:  t?.TrafficLast24hBytes ?? 0
            );
        }).ToList();

        return PagedResponseDto<AdminUserCardDto>.From(
            items,
            baseResult.Page,
            baseResult.PageSize,
            baseResult.Total
        );
    }
}