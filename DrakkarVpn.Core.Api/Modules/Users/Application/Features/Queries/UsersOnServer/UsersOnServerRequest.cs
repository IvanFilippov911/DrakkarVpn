using DrakkarVpn.Core.Api.Modules.Subscriptions.Domain.ValueObjects;
using DrakkarVpn.Core.Api.Modules.Users.Application.DTOs;
using DrakkarVpn.Core.Api.Modules.Users.Domain;
using DrakkarVpn.Shared;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Users.Application.Features.Queries.GetListServerUsers;

public sealed record UsersOnServerRequest(
    Guid ServerId,
    int Page = 1,
    int PageSize = 25,
    string? Search = null,
    UserStatus? Status = null,
    SubscriptionStatus? SubscriptionStatus = null,
    UsersSortBy SortBy = UsersSortBy.EndAtDesc
) : IRequest<PagedResponseDto<UserCardDto>>;