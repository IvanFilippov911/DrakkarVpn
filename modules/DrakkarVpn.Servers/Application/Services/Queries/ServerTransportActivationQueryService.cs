using DrakkarVpn.Core.Api.Modules.Servers.Application.Abstractions;
using DrakkarVpn.Servers.Application.Abstractions.Services.ServerTransportActivations;
using DrakkarVpn.Servers.Application.DTOs.ServerTransportActivations;
using DrakkarVpn.Servers.Application.Errors;
using DrakkarVpn.Shared.Errors.DomainErrors;

namespace DrakkarVpn.Servers.Application.Services.Queries;

public sealed class ServerTransportActivationQueryService : IServerTransportActivationQueryService
{
    private readonly IServerTransportActivationReadRepository _readRepository;

    public ServerTransportActivationQueryService(IServerTransportActivationReadRepository readRepository)
        => _readRepository = readRepository;

    public async Task<IReadOnlyList<ServerTransportActivationListItemDto>> GetByServerAsync(
        Guid serverId,
        CancellationToken ct)
    {
        if (!await _readRepository.ServerExistsAsync(serverId, ct))
        {
            throw new DomainException(
                DomainArea.Servers,
                ServerTransportActivationErrorCodes.ServerNotFound,
                $"Server '{serverId}' was not found.");
        }

        return await _readRepository.GetByServerAsync(serverId, ct);
    }
}
