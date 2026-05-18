using DrakkarVpn.Core.Api.Modules.Servers.Application.Abstractions;
using DrakkarVpn.Servers.Application.Abstractions.Repositories;
using DrakkarVpn.Servers.Application.Abstractions.Services.Queries;
using DrakkarVpn.Servers.Application.DTOs.ServerTransportApplyJobs;
using Microsoft.EntityFrameworkCore;

namespace DrakkarVpn.Servers.Application.Services.Queries;

public sealed class ServerTransportStateQueryService : IServerTransportStateQueryService
{
    private readonly IServerRepository _servers;

    public ServerTransportStateQueryService(IServerRepository servers)
        => _servers = servers;

    public async Task<Dictionary<Guid, ServerTransportDesiredStateDto>> GetTransportDesiredStatesAsync(
        Guid[] serverIds,
        CancellationToken ct)
    {
        if (serverIds.Length == 0)
            return [];

        return await _servers.Query()
            .Where(s => serverIds.Contains(s.Id))
            .Select(s => new
            {
                s.Id,
                DesiredState = new ServerTransportDesiredStateDto(
                    s.DesiredTransportActivationId,
                    s.DesiredTransportVersion)
            })
            .ToDictionaryAsync(x => x.Id, x => x.DesiredState, ct);
    }
}
