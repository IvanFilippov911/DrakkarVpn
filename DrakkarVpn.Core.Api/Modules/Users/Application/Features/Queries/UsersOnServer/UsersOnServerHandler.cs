using DrakkarVpn.Core.Api.Modules.Subscriptions.Application.Features.Queries.SubscriptionsSummariesForUsers;
using DrakkarVpn.Core.Api.Modules.Users.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Users.Application.DTOs;
using DrakkarVpn.Shared;
using DrakkarVpn.Shared.Subscriptions;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Users.Application.Features.Queries.GetListServerUsers;

public sealed class ListServerUsersQueryHandler
    : IRequestHandler<UsersOnServerRequest, PagedResponseDto<UserCardDto>>
{
    private readonly IAppUserRepository _usersRepo; 
    private readonly IMediator _mediator;           

    public ListServerUsersQueryHandler(IAppUserRepository usersRepo, IMediator mediator)
    {
        _usersRepo = usersRepo;
        _mediator = mediator;
    }

    public async Task<PagedResponseDto<UserCardDto>> Handle(UsersOnServerRequest req, CancellationToken ct)
    {
        var (rows, total) = await _usersRepo.SearchOnServerAsync(
            req.ServerId, req.Page, req.PageSize, req.Search, req.Status, req.SubscriptionStatus, req.SortBy, ct);

        if (rows.Count == 0)
            return PagedResponseDto<UserCardDto>.Empty(req.Page, req.PageSize);

        var userIds = rows.Select(x => x.UserId).ToArray();
        
        var subsByUser 
            = await _mediator.Send(new SubscriptionsSummariesForUsersQuery(userIds), ct);
        
        var items = rows.Select(u =>
        {
            subsByUser.TryGetValue(u.UserId, out var sub);
            
            return new UserCardDto(
                new UserSummaryDto(u.UserId, u.Telegram, u.CreatedAtUtc, u.Status),
                new SubscriptionSummaryDto(
                    sub?.Id ?? Guid.Empty,
                    sub?.IsActive ?? false,
                    sub?.EndAtUtc ?? DateTime.UnixEpoch,
                    sub?.MaxDevices ?? 0));
        }).ToList();
        
        return PagedResponseDto<UserCardDto>.From(items, req.Page, req.PageSize, total);
    }
}