using DrakkarVpn.Core.Api.Modules.Users.Application.DTOs;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Users.Application.Features.Queries.GetUsersTrafficSummary;

public sealed record GetUsersTrafficSummaryRequest(
    Guid     ServerId,
    Guid[]   UserIds,
    DateTime From24hUtc
) : IRequest<Dictionary<Guid, UsersTrafficSummaryDto>>;