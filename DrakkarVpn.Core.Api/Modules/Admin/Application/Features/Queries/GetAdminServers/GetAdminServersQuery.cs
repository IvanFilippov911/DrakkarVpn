using DrakkarVpn.Core.Api.Modules.Admin.Application.DTOs;
using DrakkarVpn.Shared;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Queries.GetAdminServers;

public sealed record GetAdminServersQuery(
    string? Region,
    string? Status,
    int Page = 1,
    int PageSize = 25
) : IRequest<PagedResponseDto<AdminServerCardDto>>;