using DrakkarVpn.Core.Api.Modules.Subscriptions.Domain.ValueObjects;
using DrakkarVpn.Core.Api.Modules.Users.Domain;

namespace DrakkarVpn.Core.Api.Modules.Admin.API.Contracts;

public sealed record GetAdminServerUsersApiQuery(
    int    Page,
    int    PageSize,
    string? Search = null,
    UserStatus? Status = null,
    SubscriptionStatus? SubscriptionStatus = null,
    UsersSortBy SortBy = UsersSortBy.EndAtDesc
);