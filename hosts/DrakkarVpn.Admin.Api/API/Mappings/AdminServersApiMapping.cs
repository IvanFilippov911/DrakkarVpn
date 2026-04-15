using DrakkarVpn.Admin.Api.API.Contracts.Servers;
using DrakkarVpn.Admin.Api.Application.Features.Commands.Servers.RegisterServer;
using DrakkarVpn.Core.Api.Modules.Admin.API.Contracts.Servers;
using DrakkarVpn.Core.Api.Modules.Admin.Application.DTOs;
using DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Commands.Servers.RegisterServer;
using DrakkarVpn.Core.Api.Modules.Peers.Application.DTOs;
using DrakkarVpn.Core.Api.Modules.Servers.Application.Features.Queries.GetServers;
using DrakkarVpn.Shared;

namespace DrakkarVpn.Core.Api.Modules.Admin.API.Mappings;

public static class AdminServersApiMapping
{
    public static AdminServerCardApiResponse ToApiResponse(this AdminServerCardDto dto)
    {
        return new AdminServerCardApiResponse(
            Id:                  dto.Id,
            Name:                dto.Name,
            Region:              dto.Region,
            Status:              dto.Status,
            Reachable:           dto.Reachable,
            OnlinePeers:         dto.OnlinePeers,
            PeersActive:         dto.PeersActive,
            MaxPeers:            dto.MaxPeers,
            VpnSpeedMbps:        dto.VpnSpeedMbps,
            InfraLatencyMs:      dto.InfraLatencyMs,
            TrafficLast1hBytes:  dto.TrafficLast1hBytes,
            TrafficLast24hBytes: dto.TrafficLast24hBytes
        );
    }

    public static PagedResponseDto<AdminServerCardApiResponse> ToApiResponse(
        this PagedResponseDto<AdminServerCardDto> page)
    {
        var items = page.Items.Select(ToApiResponse).ToList();

        return new PagedResponseDto<AdminServerCardApiResponse>(
            Items:      items,
            Page:       page.Page,
            PageSize:   page.PageSize,
            Total:      page.Total,
            TotalPages: page.TotalPages);
    }

    public static ServerDetailsApiResponse ToApiResponse(this GetServersDetailDto dto)
    {
        return new ServerDetailsApiResponse(
            Id:              dto.Id,
            Name:            dto.Name,
            Region:          dto.Region,
            PublicHost:      dto.PublicHost,
            AgentBaseUrl:   dto.AgentBaseUrl,
            Status:         dto.Status,
            Reachable:      dto.Reachable,
            PeersActive:    dto.PeersActive,
            MaxPeers:       dto.MaxPeers,
            TrafficRxBytes: dto.TrafficRxBytes,
            TrafficTxBytes: dto.TrafficTxBytes,
            VpnSpeedMbps:   dto.VpnSpeedMbps,
            InfraLatencyMs: dto.InfraLatencyMs);
    }

    public static ServerMetricsHistoryApiResponse ToApiResponse(this ServerMetricsHistoryDto dto)
    {
        return new ServerMetricsHistoryApiResponse(
            PeriodStartUtc: dto.PeriodStartUtc,
            ServerId:       dto.ServerId,
            Reachable:      dto.Reachable,
            TrafficRxBytes: dto.TrafficRxBytes,
            TrafficTxBytes: dto.TrafficTxBytes,
            VpnSpeedMbps:   dto.VpnSpeedMbps,
            InfraLatencyMs: dto.InfraLatencyMs);
    }

    public static IReadOnlyList<ServerMetricsHistoryApiResponse> ToApiResponse(
        this IReadOnlyList<ServerMetricsHistoryDto> items)
    {
        return items.Select(ToApiResponse).ToList();
    }

    public static ServerOnlinePointApiResponse ToApiResponse(this ServerOnlinePointDto dto)
    {
        return new ServerOnlinePointApiResponse(
            PeriodStartUtc: dto.PeriodStartUtc,
            OnlinePeers:    dto.OnlinePeers);
    }

    public static IReadOnlyList<ServerOnlinePointApiResponse> ToApiResponse(
        this IReadOnlyList<ServerOnlinePointDto> items)
    {
        return items.Select(ToApiResponse).ToList();
    }

    public static ServerPeerApiResponse ToApiResponse(this ServerPeerDto dto)
    {
        return new ServerPeerApiResponse(
            PeerId:             dto.PeerId,
            UserId:             dto.UserId,
            Status:             dto.Status.ToString(),
            IsOnline:           dto.IsOnline,
            LastDataAtUtc:      dto.LastDataAtUtc,
            TrafficLast1hBytes: dto.TrafficLast1hBytes,
            TrafficLast24hBytes: dto.TrafficLast24hBytes,
            SpeedMbps:          dto.SpeedMbps,
            VpnLatencyMs:       dto.VpnLatencyMs,
            CreatedAtUtc:       dto.CreatedAtUtc);
    }

    public static PagedResponseDto<ServerPeerApiResponse> ToApiResponse(
        this PagedResponseDto<ServerPeerDto> page)
    {
        var items = page.Items.Select(ToApiResponse).ToList();

        return new PagedResponseDto<ServerPeerApiResponse>(
            Items:      items,
            Page:       page.Page,
            PageSize:   page.PageSize,
            Total:      page.Total,
            TotalPages: page.TotalPages);
    }

    public static RegisterServerRequest ToCommand(this RegisterServerApiRequest request)
    {
        return new RegisterServerRequest(
            Name: request.Name,
            Region: request.Region,
            PublicHost: request.PublicHost,
            PublicPort: request.PublicPort,
            AgentBaseUrl: request.AgentBaseUrl,
            AgentTokenEncrypted: request.AgentTokenEncrypted,
            MaxPeers: request.MaxPeers
        );
    }
}

