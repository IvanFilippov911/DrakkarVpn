using DrakkarVpn.Core.Api.Modules.Admin.Application.DTOs;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Queries.GetServersOverview;

public sealed record GetServersOverviewQuery
    : IRequest<ServersWithTrafficOverviewDto>;