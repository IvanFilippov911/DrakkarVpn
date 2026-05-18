using DrakkarVpn.Core.Api.Modules.Servers.Application.Features.Queries.GetServers;
using DrakkarVpn.Core.Api.Modules.Servers.Domain;
using DrakkarVpn.Shared;

namespace DrakkarVpn.Servers.Application.Abstractions.Services.Queries;

public interface IServersQueryService
{
    Task<PagedResponseDto<GetServerDto>> GetPagedAsync(
        string? region,
        ServerStatus? status,
        int page,
        int pageSize,
        CancellationToken ct);

    Task<GetServersDetailDto?> GetDetailAsync(Guid serverId, CancellationToken ct);
}
