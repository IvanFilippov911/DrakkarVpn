using DrakkarVpn.Core.Api.Modules.Admin.Application.DTOs;
using DrakkarVpn.Core.Api.Modules.Servers.Domain;
using DrakkarVpn.Shared;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Queries.GetAdminServers;

public sealed record GetAdminServersQuery(
    string? Region,
    ServerStatus? Status,
    int Page = 1,
    int PageSize = 25
) : IRequest<PagedResponseDto<AdminServerCardDto>>;