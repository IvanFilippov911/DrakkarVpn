using DrakkarVpn.Admin.Api.API.Contracts.TransportProfiles;
using DrakkarVpn.Admin.Api.Application.Features.Commands.TransportProfiles.CreateTransportProfile;
using DrakkarVpn.Admin.Api.Application.Features.Commands.TransportProfiles.UpdateTransportProfile;
using DrakkarVpn.Servers.Application.DTOs.TransportProfiles;
using DrakkarVpn.Shared;

namespace DrakkarVpn.Admin.Api.API.Mappings;

public static class AdminTransportProfilesApiMapping
{
    public static CreateTransportProfileRequest ToCommand(this CreateTransportProfileApiRequest request)
        => new(new CreateTransportProfileDto(
            Name: request.Name,
            TransportType: request.TransportType,
            SecurityType: request.SecurityType,
            RealitySni: request.RealitySni,
            RealityShortId: request.RealityShortId,
            RealityFingerprint: request.RealityFingerprint,
            RealityDest: request.RealityDest,
            GrpcServiceName: request.GrpcServiceName,
            GrpcAuthority: request.GrpcAuthority,
            GlobalPriority: request.GlobalPriority));

    public static UpdateTransportProfileRequest ToCommand(
        this UpdateTransportProfileApiRequest request,
        Guid profileId)
        => new(
            ProfileId: profileId,
            Data: new UpdateTransportProfileDto(
                Name: request.Name,
                RealitySni: request.RealitySni,
                RealityShortId: request.RealityShortId,
                RealityFingerprint: request.RealityFingerprint,
                RealityDest: request.RealityDest,
                GrpcServiceName: request.GrpcServiceName,
                GrpcAuthority: request.GrpcAuthority,
                GlobalPriority: request.GlobalPriority));

    public static TransportProfileSortBy ToDomainSortBy(this TransportProfileSortByApi sortBy)
        => sortBy switch
        {
            TransportProfileSortByApi.UpdatedAtUtc => TransportProfileSortBy.UpdatedAtUtc,
            _ => TransportProfileSortBy.GlobalPriority
        };

    public static TransportProfileApiResponse ToApiResponse(this TransportProfileDto dto)
        => new(
            Id: dto.Id,
            Name: dto.Name,
            TransportType: dto.TransportType,
            SecurityType: dto.SecurityType,
            RealitySni: dto.RealitySni,
            RealityShortId: dto.RealityShortId,
            RealityFingerprint: dto.RealityFingerprint,
            RealityDest: dto.RealityDest,
            GrpcServiceName: dto.GrpcServiceName,
            GrpcAuthority: dto.GrpcAuthority,
            GlobalPriority: dto.GlobalPriority,
            IsEnabled: dto.IsEnabled,
            CreatedAtUtc: dto.CreatedAtUtc,
            UpdatedAtUtc: dto.UpdatedAtUtc,
            Version: dto.Version);

    public static PagedResponseDto<TransportProfileApiResponse> ToApiResponse(
        this PagedResponseDto<TransportProfileDto> page)
    {
        var items = page.Items.Select(ToApiResponse).ToList();
        return new PagedResponseDto<TransportProfileApiResponse>(
            Items: items,
            Page: page.Page,
            PageSize: page.PageSize,
            Total: page.Total,
            TotalPages: page.TotalPages);
    }
}
