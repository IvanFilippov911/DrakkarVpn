using DrakkarVpn.Core.Api.Modules.Admin.Application.DTOs;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Queries.HealthSystem.GetCoreHealth;

public sealed record GetCoreHealthQuery : IRequest<CoreHealthDto>;