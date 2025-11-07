namespace DrakkarVpn.Core.Api.Modules.Users.Application.DTOs;

public sealed record UsersTrafficSummaryDto(
    Guid UserId,
    long TrafficLast24hBytes
);