using DrakkarVpn.Servers.Application.Abstractions.Repositories;
using DrakkarVpn.Servers.Application.Abstractions.Services;
using DrakkarVpn.Servers.Application.Abstractions.Services.ServerTransportActivations;
using DrakkarVpn.Servers.Application.DTOs.ServerTransportActivations;
using DrakkarVpn.Servers.Domain.Exceptions;

namespace DrakkarVpn.Servers.Application.Services.ServerTransportActivations;

public sealed class ServerTransportActivationService : IServerTransportActivationService
{
    private readonly IServerRepository _serverRepository;
    private readonly IDateTimeProvider _dateTimeProvider;

    public ServerTransportActivationService(
        IServerRepository serverRepository,
        IDateTimeProvider dateTimeProvider)
    {
        _serverRepository = serverRepository;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<long> ActivateAsync(ActivateServerTransportActivationInput input, CancellationToken ct)
    {
        input = input ?? throw new ArgumentNullException(nameof(input));

        var server = await _serverRepository.GetForTransportActivationUpdateAsync(input.ServerId, ct)
            ?? throw ServerTransportActivationErrors.ServerNotFound(input.ServerId);

        var now = _dateTimeProvider.UtcNow;
        try
        {
            server.ActivateTransportActivation(input.ActivationId, now);
        }
        catch (TransportActivationNotAttachedException)
        {
            throw ServerTransportActivationErrors.ActivationNotFound(input.ServerId, input.ActivationId);
        }

        return server.DesiredTransportVersion;
    }
}
