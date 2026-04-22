using DrakkarVpn.Admin.Api.API.Contracts.NetworkMonitoring;
using DrakkarVpn.Admin.Api.Application.Features.Commands.NetworkMonitoring.RegisterProbeNode;
using DrakkarVpn.Admin.Api.Application.Features.Commands.NetworkMonitoring.UpdateProbeNode;
using NetworkMonitoring.Application.DTOs;

namespace DrakkarVpn.Admin.Api.API.Mappings;

public static class AdminNetworkMonitoringApiMapping
{
    public static RegisterProbeNodeRequest ToCommand(this RegisterProbeNodeApiRequest request)
        => new(new RegisterProbeNodeDto(
            Name: request.Name,
            Region: request.Region,
            Host: request.Host));

    public static UpdateProbeNodeRequest ToCommand(
        this UpdateProbeNodeApiRequest request,
        Guid probeNodeId)
        => new(
            ProbeNodeId: probeNodeId,
            Data: new RegisterProbeNodeDto(
                Name: request.Name,
                Region: request.Region,
                Host: request.Host));

    public static ProbeNodeApiResponse ToApiResponse(this ProbeNodeDto dto)
        => new(
            Id: dto.Id,
            Name: dto.Name,
            Region: dto.Region,
            Host: dto.Host,
            Status: dto.Status,
            IsEnabled: dto.IsEnabled,
            LastSeenAtUtc: dto.LastSeenAtUtc,
            CreatedAtUtc: dto.CreatedAtUtc,
            UpdatedAtUtc: dto.UpdatedAtUtc);

    public static IReadOnlyList<ProbeNodeApiResponse> ToApiResponse(
        this IReadOnlyList<ProbeNodeDto> items)
        => items.Select(ToApiResponse).ToList();
}

