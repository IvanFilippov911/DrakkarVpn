using DrakkarVpn.Core.Api.Modules.Admin.Application.DTOs;
using DrakkarVpn.Core.Api.Modules.Subscriptions.Domain.ValueObjects;
using DrakkarVpn.Core.Api.Modules.Users.Domain;
using DrakkarVpn.Shared;
using DrakkarVpn.Shared.Subscriptions;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Queries.GetAdminServerUsers;

public sealed record GetAdminUsersQuery(
    int    Page,
    int    PageSize,
    string? Search = null,
    UserStatus? Status = null,
    SubscriptionStatus? SubscriptionStatus = null,
    UsersSortBy SortBy = UsersSortBy.CreatedAt,
    UserSortDirection UserSortDirection = 0
) : IRequest<PagedResponseDto<AdminUserCardDto>>;