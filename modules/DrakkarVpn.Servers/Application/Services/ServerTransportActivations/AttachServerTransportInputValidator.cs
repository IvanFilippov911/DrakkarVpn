using DrakkarVpn.Core.Api.Modules.Servers.Application.Abstractions;
using DrakkarVpn.Servers.Application.Abstractions.Repositories;
using DrakkarVpn.Servers.Application.DTOs.ServerTransportActivations;
using DrakkarVpn.Servers.Application.Errors;
using DrakkarVpn.Shared.Errors.DomainErrors;

namespace DrakkarVpn.Servers.Application.Services.ServerTransportActivations;

internal static class AttachServerTransportInputValidator
{
    public static HashSet<Guid> ValidateAndExtractAttachProfileIds(AttachServerTransportProfilesInput input)
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

    public static async Task EnsureProfilesCanBeAttachedAsync(
        IServerTransportActivationReadRepository readRepository,
        IReadOnlyCollection<Guid> profileIds,
        CancellationToken ct)
    {
        var attachCandidates = await readRepository.GetTransportProfilesForAttachAsync(profileIds, ct);
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
}
