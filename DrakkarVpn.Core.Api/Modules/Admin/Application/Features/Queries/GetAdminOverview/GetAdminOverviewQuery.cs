using DrakkarVpn.Core.Api.Modules.Admin.Application.DTOs;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Queries.GetAdminOverview;

public sealed record GetAdminOverviewQuery : IRequest<AdminOverviewDto>;
