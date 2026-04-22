using DrakkarVpn.Core.Api.Modules.Servers.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Servers.Domain.Exceptions;
using DrakkarVpn.Servers.Application.Abstractions.Services;
using DrakkarVpn.Servers.Application.DTOs.ServerTransportActivations;
using DrakkarVpn.Servers.Application.Errors;
using DrakkarVpn.Shared.Errors.DomainErrors;

namespace DrakkarVpn.Servers.Application.Services;

public sealed class ServerTransportActivationManagementService : IServerTransportActivationManagementService
{
    private readonly IServerRepository _serverRepository;
    private readonly IServerTransportActivationReadRepository _readRepository;
    private readonly IDateTimeProvider _dateTimeProvider;

    public ServerTransportActivationManagementService(
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
        var server = await GetServerOrThrowAsync(input.ServerId, ct);

        var profileSet = ValidateAndExtractAttachProfileIds(input);
        if (profileSet.Count == 0)
            return;

        await EnsureProfilesCanBeAttachedAsync(profileSet, ct);

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

    public async Task<IReadOnlyList<ServerTransportActivationListItemDto>> GetByServerAsync(Guid serverId, CancellationToken ct)
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

    public async Task UpdateAsync(UpdateServerTransportActivationInput input, CancellationToken ct)
    {
        input = input ?? throw new ArgumentNullException(nameof(input));
        var server = await GetServerOrThrowAsync(input.ServerId, ct);

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
            throw BuildActivationNotFoundException(input.ServerId, input.ActivationId);
        }
    }

    public async Task ActivateAsync(ActivateServerTransportActivationInput input, CancellationToken ct)
    {
        input = input ?? throw new ArgumentNullException(nameof(input));
        var server = await GetServerOrThrowAsync(input.ServerId, ct);

        var now = _dateTimeProvider.UtcNow;
        try
        {
            server.ActivateTransportActivation(input.ActivationId, now);
        }
        catch (TransportActivationNotAttachedException)
        {
            throw BuildActivationNotFoundException(input.ServerId, input.ActivationId);
        }
    }

    public async Task DetachAsync(DetachServerTransportActivationInput input, CancellationToken ct)
    {
        input = input ?? throw new ArgumentNullException(nameof(input));
        var server = await GetServerOrThrowAsync(input.ServerId, ct);

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
            throw BuildActivationNotFoundException(input.ServerId, input.ActivationId);
        }
    }

    private async Task<Core.Api.Modules.Servers.Domain.Server> GetServerOrThrowAsync(Guid serverId, CancellationToken ct)
    {
        var server = await _serverRepository.GetAsync(serverId, ct);
        if (server is not null)
            return server;

        throw new DomainException(
            DomainArea.Servers,
            ServerTransportActivationErrorCodes.ServerNotFound,
            $"Server '{serverId}' was not found.");
    }

    private static HashSet<Guid> ValidateAndExtractAttachProfileIds(AttachServerTransportProfilesInput input)
    {
        var profiles = input.Profiles;
        if (profiles.Count == 0)
        {
            if (input.ActivateProfileId.HasValue)
            {
                throw new DomainException(
                    DomainArea.Servers,
                    ServerTransportActivationErrorCodes.ActivateProfileIdIsNotInInput,
                    $"Activate profile '{input.ActivateProfileId.Value}' must be part of the attached profile list.");
            }

            return new HashSet<Guid>();
        }

        var duplicateGroup = profiles
            .GroupBy(x => x.TransportProfileId)
            .FirstOrDefault(x => x.Count() > 1);

        if (duplicateGroup is not null)
        {
            throw new DomainException(
                DomainArea.Servers,
                ServerTransportActivationErrorCodes.DuplicateTransportProfileIdsInInput,
                $"Transport profile '{duplicateGroup.Key}' is duplicated in input.");
        }

        var profileSet = profiles
            .Select(x => x.TransportProfileId)
            .ToHashSet();

        if (input.ActivateProfileId.HasValue && !profileSet.Contains(input.ActivateProfileId.Value))
        {
            throw new DomainException(
                DomainArea.Servers,
                ServerTransportActivationErrorCodes.ActivateProfileIdIsNotInInput,
                $"Activate profile '{input.ActivateProfileId.Value}' must be part of the attached profile list.");
        }

        return profileSet;
    }

    private async Task EnsureProfilesCanBeAttachedAsync(IReadOnlyCollection<Guid> profileIds, CancellationToken ct)
    {
        var attachCandidates = await _readRepository.GetTransportProfilesForAttachAsync(profileIds, ct);
        if (attachCandidates.Count != profileIds.Count)
        {
            var missingProfileId = profileIds.Except(attachCandidates.Select(x => x.TransportProfileId)).First();
            throw new DomainException(
                DomainArea.Servers,
                ServerTransportActivationErrorCodes.TransportProfileNotFound,
                $"Transport profile '{missingProfileId}' was not found.");
        }

        var disabledProfile = attachCandidates.FirstOrDefault(x => !x.IsEnabled);
        if (disabledProfile is not null)
        {
            throw new DomainException(
                DomainArea.Servers,
                ServerTransportActivationErrorCodes.TransportProfileDisabled,
                $"Transport profile '{disabledProfile.TransportProfileId}' is disabled and cannot be attached.");
        }
    }

    private static DomainException BuildActivationNotFoundException(Guid serverId, Guid activationId)
    {
        return new DomainException(
            DomainArea.Servers,
            ServerTransportActivationErrorCodes.ActivationNotFound,
            $"Activation '{activationId}' was not found on server '{serverId}'.");
    }
}
