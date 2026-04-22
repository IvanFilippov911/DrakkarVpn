using DrakkarVpn.Servers.Application.DTOs.TransportProfiles;
using DrakkarVpn.Servers.Domain.Enums;
using DrakkarVpn.Shared;
using DrakkarVpn.Shared.Servers;
using MediatR;

namespace DrakkarVpn.Admin.Api.Application.Features.Queries.TransportProfiles.GetAdminTransportProfiles;

public sealed record GetAdminTransportProfilesQuery(
    int Page = 1,
    int PageSize = 25,
    string? Search = null,
    bool? IsEnabled = null,
    TransportType? TransportType = null,
    TransportProfileSortBy SortBy = TransportProfileSortBy.GlobalPriority,
    SortDirection SortDirection = SortDirection.Asc
) : IRequest<PagedResponseDto<TransportProfileDto>>;
