using DrakkarVpn.Core.Api.Modules.Peers.Application.DTOs;
using DrakkarVpn.Core.Api.Modules.Servers.Application.Features.Queries.GetServers;
using DrakkarVpn.Shared;
using DrakkarVpn.Shared.Peers;
using DrakkarVpn.Shared.Servers;

namespace DrakkarVpn.Core.Api.Modules.Peers.Application.Abstractions;

public interface IPeersQueryService
{
    // ---------------------------
    // 1) Counters / quick stats
    // ---------------------------
    Task<int> CountPeersOnServerAsync(Guid serverId, CancellationToken ct);
    Task<int> GetActivePeersCountAsync(Guid serverId, CancellationToken ct);
    Task<Dictionary<Guid, int>> GetServersOnlinePeersSummaryAsync(Guid[] serverIds, CancellationToken ct);

    // ---------------------------
    // 2) Peer reads (lookup)
    // ---------------------------
    Task<ActivePeerForDeviceDto?> GetActivePeerForDeviceAsync(string deviceId, CancellationToken ct);
    Task<PeerDataForConfigDto?> GetDataForConfigByDeviceIdAsync(string deviceId, CancellationToken ct);
    Task<PeerDataForConfigDto?> GetDataForConfigByAgentUuidAsync(Guid agentUuid, CancellationToken ct);
    Task<PeerResponseDto> GetPeerByIdAsync(Guid peerId, CancellationToken ct);
    Task<IReadOnlyList<PeerForRevokeDto>> GetPeersForRevokeAsync(Guid userId, CancellationToken ct);

    // ---------------------------
    // 3) Peer analytics / details
    // ---------------------------
    Task<PeerDetailsDto?> GetPeerDetailsAsync(Guid peerId, CancellationToken ct);

    Task<long> GetPeerTrafficForPeriodAsync(
        Guid peerId,
        DateTime fromUtc,
        DateTime toUtc,
        CancellationToken ct);

    Task<IReadOnlyList<PeerMetricsHistoryDto>> GetPeerHistoryAsync(
        Guid peerId,
        DateTime? fromUtc,
        DateTime? toUtc,
        CancellationToken ct);

    // ---------------------------
    // 4) Server peers views
    // ---------------------------
    Task<PagedResponseDto<ServerPeerDto>> GetServerPeersAsync(
        Guid serverId,
        int page,
        int pageSize,
        bool onlyOnline,
        Guid? peerId,
        ServerPeerSortBy? sortBy,
        SortDirection? direction,
        CancellationToken ct);

    Task<IReadOnlyList<ServerOnlinePointDto>> GetServerPeersOnlineHistoryAsync(
        Guid serverId,
        int minutes,
        CancellationToken ct);
}