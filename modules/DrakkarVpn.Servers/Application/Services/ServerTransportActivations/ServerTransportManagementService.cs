using DrakkarVpn.Core.Api.Modules.Servers.Application.Abstractions;
using DrakkarVpn.Servers.Application.Abstractions.Repositories;
using DrakkarVpn.Servers.Application.Abstractions.Services;
using DrakkarVpn.Servers.Application.Abstractions.Services.ServerTransportActivations;
using DrakkarVpn.Servers.Application.DTOs.ServerTransportActivations;
using DrakkarVpn.Servers.Application.Errors;
using DrakkarVpn.Servers.Domain.Exceptions;
using DrakkarVpn.Shared.Errors.DomainErrors;

namespace DrakkarVpn.Servers.Application.Services.ServerTransportActivations;

public sealed class ServerTransportManagementService : IServerTransportManagementService
{
    private readonly IServerRepository _serverRepository;
    private readonly IServerTransportActivationReadRepository _readRepository;
    private readonly IDateTimeProvider _dateTimeProvider;

    public ServerTransportManagementService(
        IServerRepository serverRepository,
        IServerTransportActivationReadRepository readRepository,
        IDateTimeProvider dateTimeProvider)
    {
        _serverRepository = serverRepository;
        _readRepository = readRepository;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task AttachProfilesAsync(AttachServerTransportProfilesInput input, CancellationToken ct)
    {
        input = input ?? throw new ArgumentNullException(nameof(input));

        var server = await _serverRepository.GetAsync(input.ServerId, ct)
            ?? throw ServerTransportActivationErrors.ServerNotFound(input.ServerId);

        var profileSet = AttachServerTransportInputValidator.ValidateAndExtractAttachProfileIds(input);
        if (profileSet.Count == 0)
            return;

        await AttachServerTransportInputValidator.EnsureProfilesCanBeAttachedAsync(
            _readRepository,
            profileSet,
            ct);

        var now = _dateTimeProvider.UtcNow;
        var activationIdsByProfileId = new Dictionary<Guid, Guid>(input.Profiles.Count);

        foreach (var profile in input.Profiles)
        {
            var activationId = Guid.NewGuid();

            try
            {
                server.AttachTransportProfile(
                    activationId: activationId,
                    transportProfileId: profile.TransportProfileId,
                    realityPublicKey: profile.RealityPublicKey,
                    localPriority: profile.LocalPriority,
                    utcNow: now.UtcDateTime);
            }
            catch (TransportProfileAlreadyAttachedException)
            {
                throw new DomainException(
                    DomainArea.Servers,
                    ServerTransportActivationErrorCodes.TransportProfileAlreadyAttached,
                    $"Transport profile '{profile.TransportProfileId}' is already attached to server '{server.Id}'.");
            }

            activationIdsByProfileId[profile.TransportProfileId] = activationId;
        }

        if (input.ActivateProfileId.HasValue)
        {
            var activationIdToActivate = activationIdsByProfileId[input.ActivateProfileId.Value];
            server.ActivateTransportActivation(activationIdToActivate, now);
        }
    }

    public async Task UpdateAsync(UpdateServerTransportActivationInput input, CancellationToken ct)
    {
        input = input ?? throw new ArgumentNullException(nameof(input));

        var server = await _serverRepository.GetAsync(input.ServerId, ct)
            ?? throw ServerTransportActivationErrors.ServerNotFound(input.ServerId);

        try
        {
            server.UpdateTransportActivation(
                activationId: input.ActivationId,
                realityPublicKey: input.RealityPublicKey,
                localPriority: input.LocalPriority,
                utcNow: _dateTimeProvider.UtcNow.UtcDateTime);
        }
        catch (TransportActivationNotAttachedException)
        {
            throw ServerTransportActivationErrors.ActivationNotFound(input.ServerId, input.ActivationId);
        }
    }

    public async Task DetachAsync(DetachServerTransportActivationInput input, CancellationToken ct)
    {
        input = input ?? throw new ArgumentNullException(nameof(input));

        var server = await _serverRepository.GetAsync(input.ServerId, ct)
            ?? throw ServerTransportActivationErrors.ServerNotFound(input.ServerId);

        try
        {
            server.DetachTransportActivation(input.ActivationId);
        }
        catch (CannotDetachActiveTransportActivationException)
        {
            throw new DomainException(
                DomainArea.Servers,
                ServerTransportActivationErrorCodes.ActiveActivationCannotBeDetached,
                $"Activation '{input.ActivationId}' is active and cannot be detached.",
                errorType: DomainErrorType.Warning);
        }
        catch (TransportActivationNotAttachedException)
        {
            throw ServerTransportActivationErrors.ActivationNotFound(input.ServerId, input.ActivationId);
        }
    }
}
