using DrakkarVpn.Core.Api.Modules.Peers.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Peers.Application.DTOs;
using DrakkarVpn.Core.Api.Modules.Peers.Application.Utils;
using DrakkarVpn.Core.Api.Modules.Servers.Application.Features.Queries.GetServers;
using DrakkarVpn.Shared;
using DrakkarVpn.Shared.Peers;
using DrakkarVpn.Shared.Servers;

namespace DrakkarVpn.Core.Api.Modules.Peers.Application.Services;

public sealed class PeersQueryService : IPeersQueryService
{
    private readonly IPeerRepository               _peers;
    private readonly IPeerMetricsHistoryRepository _history;
    private readonly IServerQueryForPeers          _serversQuery;

    public PeersQueryService(
        IPeerRepository peers,
        IPeerMetricsHistoryRepository history,
        IServerQueryForPeers serversQuery)
    {
        _peers        = peers;
        _history      = history;
        _serversQuery = serversQuery;
    }

    
    #region Counters / quick stats
    
    public Task<int> CountPeersOnServerAsync(Guid serverId, CancellationToken ct)
        => _peers.CountOnServerAsync(serverId, ct);

    public Task<int> GetActivePeersCountAsync(Guid serverId, CancellationToken ct)
        => _peers.GetActiveCountByServerIdAsync(serverId, ct);

    public Task<Dictionary<Guid, int>> GetServersOnlinePeersSummaryAsync(Guid[]? serverIds, CancellationToken ct)
    {
        if (serverIds is null || serverIds.Length == 0)
            return Task.FromResult(new Dictionary<Guid, int>());

        return _peers.GetOnlineCountsByServerAsync(serverIds, ct);
    }

    #endregion
    
    
    #region Peer reads (lookup / simple)

    public Task<ActivePeerForDeviceDto?> GetActivePeerForDeviceAsync(string deviceId, CancellationToken ct)
        => _peers.GetActiveForDeviceAsync(deviceId, ct);

    public async Task<PeerDataForConfigDto?> GetDataForConfigByDeviceIdAsync(string deviceId, CancellationToken ct)
    {
        var peer = await _peers.GetByDeviceIdAsync(deviceId, ct);
        if (peer is null) return null;

        return new PeerDataForConfigDto(
            ServerId:   peer.ServerId,
            AgentUuid: peer.AgentPeerUuid
        );
    }

    public Task<PeerDataForConfigDto?> GetDataForConfigByAgentUuidAsync(
        Guid agentUuid,
        CancellationToken ct)
        => GetDataForConfigByAgentUuidInternal(agentUuid, ct);

    private async Task<PeerDataForConfigDto?> GetDataForConfigByAgentUuidInternal(
        Guid agentUuid,
        CancellationToken ct)
    {
        var peer = await _peers.GetPeerForConfigByAgentUuidAsync(agentUuid, ct);
        if (peer is null)
            return null;

        // DTO is assembled in the service layer (not in repository).
        return new PeerDataForConfigDto(
            ServerId: peer.ServerId,
            AgentUuid: peer.AgentPeerUuid);
    }

    public async Task<PeerResponseDto> GetPeerByIdAsync(Guid peerId, CancellationToken ct)
    {
        var peer = await _peers.GetByIdAsync(peerId, ct);
        if (peer is null)
            throw new InvalidOperationException($"Peer {peerId} not found");

        return new PeerResponseDto(
            peer.Id,
            peer.DeviceId,
            peer.ServerId,
            peer.AgentPeerUuid,
            peer.Status,
            peer.CreatedAt
        );
    }

    public Task<IReadOnlyList<PeerForRevokeDto>> GetPeersForRevokeAsync(Guid userId, CancellationToken ct)
        => _peers.GetForRevokeByUserAsync(
            userId,
            p => new PeerForRevokeDto(
                p.Id,
                p.ServerId,
                (int)p.Status
            ),
            ct);

    #endregion

    
    
    #region Peer analytics / history / details
    
    public Task<long> GetPeerTrafficForPeriodAsync(
        Guid peerId,
        DateTime fromUtc,
        DateTime toUtc,
        CancellationToken ct)
        => _history.GetPeerTrafficBytesAsync(peerId, fromUtc, toUtc, ct);

    public async Task<IReadOnlyList<PeerMetricsHistoryDto>> GetPeerHistoryAsync(
        Guid peerId,
        DateTime? fromUtc,
        DateTime? toUtc,
        CancellationToken ct)
    {
        var to   = (toUtc   ?? DateTime.UtcNow).ToUniversalTime();
        var from = (fromUtc ?? to.AddHours(-24)).ToUniversalTime();

        var rows = await _history.GetRangeAsync(peerId, from, to, ct);

        return rows
            .Select(x => new PeerMetricsHistoryDto(
                PeriodStartUtc:     x.PeriodStartUtc,
                TotalRxDeltaBytes:  x.RxDeltaBytes,
                TotalTxDeltaBytes:  x.TxDeltaBytes,
                IsOnline:           x.IsOnline,
                SpeedMbps:          x.SpeedMbps,
                VpnLatencyMs:       x.VpnLatencyMs
            ))
            .ToList();
    }

    public async Task<PeerDetailsDto?> GetPeerDetailsAsync(Guid peerId, CancellationToken ct)
    {
        var row = await _peers.GetPeerAsync(peerId, ct);
        if (row is null)
            return null;
        
        var server = await _serversQuery.GetShortAsync(row.ServerId, ct);
        if (server is null)
            return null;
        
        var now     = DateTime.UtcNow;
        var from1h  = now.AddHours(-1);
        var from24h = now.AddHours(-24);

        var t1h  = await _history.GetPeerTrafficBytesAsync(row.PeerId, from1h,  now, ct);
        var t24h = await _history.GetPeerTrafficBytesAsync(row.PeerId, from24h, now, ct);

        return new PeerDetailsDto(
            Server:              server,

            PeerId:              row.PeerId,
            UserId:              row.UserId,
            Status:              row.Status,
            IsOnline:            row.IsOnline,
            LastDataAtUtc:       row.LastDataAtUtc,
            SpeedMbps:           row.SpeedMbps,
            VpnLatencyMs:        row.VpnLatencyMs,
            CreatedAtUtc:        row.CreatedAtUtc,

            TrafficLast1hBytes:  t1h,
            TrafficLast24hBytes: t24h
        );
    }

    #endregion

    
    #region Server peers views (table / online timeline)

    public async Task<PagedResponseDto<ServerPeerDto>> GetServerPeersAsync(
        Guid serverId,
        int page,
        int pageSize,
        bool onlyOnline,
        Guid? peerId,
        ServerPeerSortBy? sortBy,
        SortDirection? direction,
        CancellationToken ct)
    {
        var (rows, total) = await _peers.GetServerPeersAsync(
            serverId,
            page,
            pageSize,
            ct);

        if (rows.Count == 0)
            return PagedResponseDto<ServerPeerDto>.Empty(page, pageSize);

        var peerIds = rows.Select(r => r.PeerId).ToArray();

        var now     = DateTime.UtcNow;
        var from1h  = now.AddHours(-1);
        var from24h = now.AddHours(-24);

        var traffic1h  = await _history.GetPeersTrafficBytesOnServerAsync(serverId, peerIds, from1h,  now, ct);
        var traffic24h = await _history.GetPeersTrafficBytesOnServerAsync(serverId, peerIds, from24h, now, ct);

        var items = rows.Select(r =>
        {
            traffic1h.TryGetValue(r.PeerId,  out var t1);
            traffic24h.TryGetValue(r.PeerId, out var t24);

            return new ServerPeerDto(
                PeerId:              r.PeerId,
                UserId:              r.UserId,
                Status:              r.Status,
                IsOnline:            r.IsOnline,
                LastDataAtUtc:       r.LastDataAtUtc,
                TrafficLast1hBytes:  t1,
                TrafficLast24hBytes: t24,
                SpeedMbps:           r.SpeedMbps,
                VpnLatencyMs:        r.VpnLatencyMs,
                CreatedAtUtc:        r.CreatedAtUtc
            );
        });

        items = ServerPeersQueryUtils.ApplyFilters(
            items,
            onlyOnline,
            peerId);

        items = ServerPeersQueryUtils.ApplySorting(
            items,
            sortBy,
            direction);

        var list = items.ToList();

        return PagedResponseDto<ServerPeerDto>.From(
            list,
            page,
            pageSize,
            total);
    }

    public Task<IReadOnlyList<ServerOnlinePointDto>> GetServerPeersOnlineHistoryAsync(
        Guid serverId,
        int minutes,
        CancellationToken ct)
    {
        var toUtc   = DateTime.UtcNow;
        var fromUtc = toUtc.AddMinutes(-minutes);

        return _history.GetServerPeersOnlineTimelineAsync(serverId, fromUtc, toUtc, ct);
    }

    #endregion
}