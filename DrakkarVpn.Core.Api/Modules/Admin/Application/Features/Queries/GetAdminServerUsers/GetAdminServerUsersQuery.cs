using DrakkarVpn.Core.Api.Modules.Admin.Application.DTOs;
using DrakkarVpn.Core.Api.Modules.Subscriptions.Domain.ValueObjects;
using DrakkarVpn.Core.Api.Modules.Users.Domain;
using DrakkarVpn.Shared;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Queries.GetAdminServerUsers;

public sealed record GetAdminServerUsersQuery(
    Guid   ServerId,
    int    Page,
    int    PageSize,
    string? Search = null,
    UserStatus? Status = null,
    SubscriptionStatus? SubscriptionStatus = null,
    UsersSortBy SortBy = UsersSortBy.EndAtDesc
) : IRequest<PagedResponseDto<AdminUserCardDto>>;