using DrakkarVpn.Core.Api.Modules.Admin.Application.DTOs;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Queries.GetUsersOverview;

public sealed record GetUsersOverviewQuery
    : IRequest<UsersOverviewDto>;