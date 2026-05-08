using DrakkarVpn.Core.Api.Modules.Servers.Application.Features.Queries.GetServers;
using DrakkarVpn.Core.Api.Modules.Servers.Domain;
using DrakkarVpn.Servers.Application.DTOs.ServerTransportApplyJobs;
using DrakkarVpn.Shared;

namespace DrakkarVpn.Core.Api.Modules.Servers.Application.Abstractions;

public interface IServersQueryService
{
    Task<IReadOnlyList<GetListServerDto>> GetListAsync(
        string? status,
        CancellationToken ct);

    Task<PagedResponseDto<GetServerDto>> GetPagedAsync(
        string? region,
        ServerStatus? status,
        int page,
        int pageSize,
        CancellationToken ct);
    
    Task<Guid[]> GetEnabledServerIdsAsync(CancellationToken ct);
    
    Task<GetServersDetailDto?> GetDetailAsync(Guid serverId, CancellationToken ct);
    Task<ServerShortDto?> GetShortAsync(Guid serverId, CancellationToken ct);
    
    Task<IReadOnlyDictionary<Guid, string>> GetAgentBaseUrlsByIdsAsync(
        IReadOnlyCollection<Guid> serverIds,
        CancellationToken ct);
    
    Task<ServerForAgentDto?> GetServerForAgentAsync(Guid serverId, CancellationToken ct);
    
    Task<IReadOnlyList<ServerMetricsHistoryDto>> GetHistoryAsync(
        Guid serverId,
        DateTime? fromUtc,
        DateTime? toUtc,
        CancellationToken ct);

    Task<IReadOnlyList<ServerMetricsHistoryDto>> GetHistoryLastAsync(
        Guid serverId,
        int minutes,
        CancellationToken ct);

    Task<Dictionary<Guid, long>> GetTrafficSummaryAsync(
        Guid[] serverIds,
        DateTime fromUtc,
        CancellationToken ct);
    
    Task<ServerConfigDataDto?> GetDataForConfigByIdAsync(
        Guid serverId,
        CancellationToken ct);

    Task<Dictionary<Guid, ServerTransportDesiredStateDto>> GetTransportDesiredStatesAsync(
        Guid[] serverIds,
        CancellationToken ct);
}