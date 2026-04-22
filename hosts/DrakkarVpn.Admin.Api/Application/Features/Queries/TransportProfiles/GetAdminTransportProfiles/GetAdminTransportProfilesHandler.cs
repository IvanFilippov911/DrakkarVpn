using DrakkarVpn.Servers.Application.Abstractions.Services;
using DrakkarVpn.Servers.Application.DTOs.TransportProfiles;
using DrakkarVpn.Shared;
using MediatR;

namespace DrakkarVpn.Admin.Api.Application.Features.Queries.TransportProfiles.GetAdminTransportProfiles;

public sealed class GetAdminTransportProfilesHandler
    : IRequestHandler<GetAdminTransportProfilesQuery, PagedResponseDto<TransportProfileDto>>
{
    private readonly ITransportProfilesManagementService _service;

    public GetAdminTransportProfilesHandler(ITransportProfilesManagementService service)
        => _service = service;

    public Task<PagedResponseDto<TransportProfileDto>> Handle(
        GetAdminTransportProfilesQuery query,
        CancellationToken ct)
        => _service.GetPagedAsync(
            new GetTransportProfilesFilterDto(
                Page: query.Page,
                PageSize: query.PageSize,
                Search: query.Search,
                IsEnabled: query.IsEnabled,
                TransportType: query.TransportType,
                SortBy: query.SortBy,
                SortDirection: query.SortDirection),
            ct);
}
