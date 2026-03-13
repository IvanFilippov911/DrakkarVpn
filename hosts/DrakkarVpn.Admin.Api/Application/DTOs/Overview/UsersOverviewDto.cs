namespace DrakkarVpn.Core.Api.Modules.Admin.Application.DTOs;

public sealed record UsersOverviewDto(
    int TotalUsers,
    int ActiveSubscriptions,
    int ExpiringSoonDays3,
    int OnlineUsersNow
);