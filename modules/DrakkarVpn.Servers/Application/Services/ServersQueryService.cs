using DrakkarVpn.Core.Api.Modules.Servers.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Servers.Application.Features.Queries.GetServers;
using DrakkarVpn.Core.Api.Modules.Servers.Domain;
using DrakkarVpn.Core.Api.Modules.Servers.Domain.VO;
using DrakkarVpn.Servers.Application.Mappers;
using DrakkarVpn.Shared;
using DrakkarVpn.Shared.Servers;
using Microsoft.EntityFrameworkCore;

public sealed class ServersQueryService : IServersQueryService, IServerQueryForPeers
{
    private readonly IServerRepository _servers;
    private readonly IServerMetricsHistoryRepository _history;

    public ServersQueryService(
        IServerRepository servers,
        IServerMetricsHistoryRepository history)
    {
        _servers = servers;
        _history = history;
    }
    
    public async Task<IReadOnlyList<GetListServerDto>> GetListAsync(
        string? status,
        CancellationToken ct)
    {
        var q = _servers.Query();
        
        if (!string.IsNullOrWhiteSpace(status) &&
            Enum.TryParse<ServerStatus>(status, true, out var statusEnum))
        {
            q = q.Where(s => s.Status == statusEnum);
        }

        return await q
            .OrderByDescending(s => s.Health.Reachable)
            .ThenBy(s => s.Health.PeersActive)
            .Select(s => new GetListServerDto(
                s.Id,
                s.Name,
                s.Region.Code,
                s.Status.ToString(),
                s.Health.Reachable,
                s.Health.PeersActive,
                s.MaxPeers,
                s.Metrics.VpnSpeedMbps,
                s.Metrics.InfraLatencyMs,
                s.PublicHost.ToString()
            ))
            .ToListAsync(ct);
    }
    
    public async Task<PagedResponseDto<GetServerDto>> GetPagedAsync(
        string? region,
        ServerStatus? status,
        int page,
        int pageSize,
        CancellationToken ct)
    {
        page     = page     <= 0 ? 1  : page;
        pageSize = pageSize <= 0 ? 50 : pageSize;
        pageSize = Math.Clamp(pageSize, 1, 200);

        var (rows, total) = await _servers.GetPagedAsync(
            region: region,
            status: status,
            page: page,
            pageSize: pageSize,
            ct: ct);

        if (total == 0 || rows.Count == 0)
            return PagedResponseDto<GetServerDto>.Empty(page, pageSize);

        var items = rows.Select(x => x.ToGetServerDto()).ToList();

        return PagedResponseDto<GetServerDto>.From(items, page, pageSize, total);
    }
    
    public async Task<Guid[]> GetEnabledServerIdsAsync(CancellationToken ct)
    {
        return await _servers.Query()
            .AsNoTracking()
            .Where(s => s.Status == ServerStatus.Enabled)
            .Select(s => s.Id)
            .ToArrayAsync(ct);
    }

    
    public async Task<GetServersDetailDto?> GetDetailAsync(Guid serverId, CancellationToken ct)
    {
        var s = await _servers.GetAsync(serverId, ct);
        if (s is null) return null;

        return new GetServersDetailDto(
            s.Id,
            s.Name,
            s.Region.Code,
            s.PublicHost.Value,
            s.AgentBaseUrl.ToString(),
            s.Status.ToString(),
            s.Health.Reachable,
            s.Health.PeersActive,
            s.MaxPeers,
            s.Metrics.TrafficRxBytes,
            s.Metrics.TrafficTxBytes,
            s.Metrics.VpnSpeedMbps,
            s.Metrics.InfraLatencyMs
        );
    }

    public Task<ServerShortDto?> GetShortAsync(Guid serverId, CancellationToken ct)
        => _servers.GetServerShortAsync(serverId, ct);
    
    
    public Task<ServerForAgentDto?> GetServerForAgentAsync(Guid serverId, CancellationToken ct)
        => _servers.GetForAgentAsync(serverId, ct);
    
    public async Task<ServerConfigDataDto?> GetDataForConfigByIdAsync(
        Guid serverId,
        CancellationToken ct)
    {
        var server = await _servers.GetAsync(serverId, ct);
        if (server is null)
            return null;

        return new ServerConfigDataDto(
            server.Region.Code,
            server.PublicHost.Value,
            server.PublicPort,
            server.RealitySni,
            server.RealityPublicKey,
            server.RealityShortId);
    }

    
    public async Task<IReadOnlyList<ServerMetricsHistoryDto>> GetHistoryAsync(
        Guid serverId,
        DateTime? fromUtc,
        DateTime? toUtc,
        CancellationToken ct)
    {
        var to   = (toUtc   ?? DateTime.UtcNow).ToUniversalTime();
        var from = (fromUtc ?? to.AddHours(-24)).ToUniversalTime();

        var rows = await _history.GetRangeAsync(serverId, from, to, ct);

        return rows.Select(x => new ServerMetricsHistoryDto(
            x.PeriodStartUtc,
            x.ServerId,
            x.Reachable,
            x.TrafficRxDeltaBytes,
            x.TrafficTxDeltaBytes,
            x.VpnSpeedMbps,
            x.InfraLatencyMs
        )).ToList();
    }

    public Task<IReadOnlyList<ServerMetricsHistoryDto>> GetHistoryLastAsync(
        Guid serverId,
        int minutes,
        CancellationToken ct)
    {
        minutes = minutes <= 0 ? 60 : minutes;
        var to = DateTime.UtcNow;
        var from = to.AddMinutes(-minutes);
        return GetHistoryAsync(serverId, from, to, ct);
    }

    public Task<Dictionary<Guid, long>> GetTrafficSummaryAsync(
        Guid[] serverIds,
        DateTime fromUtc,
        CancellationToken ct)
    {
        if (serverIds is null || serverIds.Length == 0)
            return Task.FromResult(new Dictionary<Guid, long>());

        return _history.GetTrafficSumAsync(serverIds, fromUtc, ct);
    }
    
    
    public async Task<IReadOnlyDictionary<Guid, string>> GetAgentBaseUrlsByIdsAsync(
        IReadOnlyCollection<Guid> serverIds,
        CancellationToken ct)
    {
        if (serverIds.Count == 0)
            return new Dictionary<Guid, string>();

        var servers = await _servers.GetByIdsAsync(serverIds.ToArray(), ct);

        return servers
            .ToDictionary(
                kv => kv.Key,
                kv => NormalizeBaseUrl(kv.Value.AgentBaseUrl!)
            );
    }

    public Task<string?> GetAgentBaseUrlAsync(Guid serverId, CancellationToken ct)
        => _servers.GetAgentBaseUrlAsync(serverId, ct);

    private static string NormalizeBaseUrl(Uri uri)
    {
        var s = uri.AbsoluteUri.TrimEnd('/');
        return s + "/";
    }
}