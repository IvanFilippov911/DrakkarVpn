using DrakkarVpn.Servers.Domain.Enums;
using DrakkarVpn.Shared.Servers;

namespace DrakkarVpn.Servers.Application.DTOs.TransportProfiles;

public sealed record GetTransportProfilesFilterDto(
    int Page,
    int PageSize,
    string? Search,
    bool? IsEnabled,
    TransportType? TransportType,
    TransportProfileSortBy SortBy,
    SortDirection SortDirection
);
