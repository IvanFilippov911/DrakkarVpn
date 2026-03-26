using DrakkarVpn.Core.Api.Modules.Servers.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Servers.Domain;
using DrakkarVpn.Core.Api.Modules.Servers.Domain.VO;
using DrakkarVpn.Servers.Application.Abstractions.Services;
using DrakkarVpn.Shared.Servers;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Servers.Application.Services;

public sealed class ServerManagementService : IServerManagementService
{
    private readonly IServerRepository _servers;
    private readonly IMediator _mediator;

    public ServerManagementService(
        IServerRepository servers,
        IMediator mediator)
    {
        _servers = servers;
        _mediator = mediator;
    }

    public async Task<Guid> RegisterAsync(
        string name,
        string region,
        string publicHost,
        int publicPort,
        string realityPublicKey,
        string realityShortId,
        string realitySni,
        string agentBaseUrl,
        string agentTokenEncrypted,
        int? maxPeers,
        CancellationToken ct)
    {
        var entity = Server.Register(
            Guid.NewGuid(),
            name,
            new Region(region),
            new PublicHost(publicHost),
            publicPort,
            realityPublicKey,
            realityShortId,
            realitySni,
            new Uri(agentBaseUrl),
            agentTokenEncrypted,
            maxPeers);

        await _servers.AddAsync(entity, ct);
        return entity.Id;
    }

    public async Task<DeleteServerResult> DeleteAsync(Guid serverId, CancellationToken ct)
    {
        var server = await _servers.GetAsync(serverId, ct);
        if (server is null)
            return DeleteServerResult.NotFound;

        await _servers.DeleteAsync(server, ct);
        return DeleteServerResult.Deleted;
    }
}