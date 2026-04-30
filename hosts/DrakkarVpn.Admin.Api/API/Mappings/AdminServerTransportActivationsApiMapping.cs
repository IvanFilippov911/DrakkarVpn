using DrakkarVpn.Admin.Api.API.Contracts.ServerTransportActivations;
using DrakkarVpn.Admin.Api.Application.Features.Commands.ServerTransportActivations.ActivateServerTransportActivation;
using DrakkarVpn.Admin.Api.Application.Features.Commands.ServerTransportActivations.AttachServerTransportActivations;
using DrakkarVpn.Admin.Api.Application.Features.Commands.ServerTransportActivations.DeleteServerTransportActivation;
using DrakkarVpn.Admin.Api.Application.Features.Commands.ServerTransportActivations.UpdateServerTransportActivation;
using DrakkarVpn.Servers.Application.DTOs.ServerTransportActivations;

namespace DrakkarVpn.Admin.Api.API.Mappings;

public static class AdminServerTransportActivationsApiMapping
{
    public static AttachServerTransportActivationsRequest ToCommand(
        this AttachServerTransportActivationsApiRequest request,
        Guid serverId)
        => new(
            ServerId: serverId,
            Profiles: request.Profiles
                .Select(x => new AttachServerTransportActivationRequestItem(
                    x.TransportProfileId,
                    x.RealityPublicKey,
                    x.LocalPriority))
                .ToList(),
            ActivateProfileId: request.ActivateProfileId);

    public static UpdateServerTransportActivationRequest ToCommand(
        this UpdateServerTransportActivationApiRequest request,
        Guid serverId,
        Guid activationId)
        => new(
            ServerId: serverId,
            ActivationId: activationId,
            RealityPublicKey: request.RealityPublicKey,
            LocalPriority: request.LocalPriority);

    public static ActivateServerTransportRequest ToCommand(Guid serverId, Guid activationId)
        => new(serverId, activationId);

    public static DeleteServerTransportActivationRequest ToCommandForDelete(Guid serverId, Guid activationId)
        => new(serverId, activationId);

    public static ServerTransportActivationApiResponse ToApiResponse(this ServerTransportActivationListItemDto dto)
        => new(
            ActivationId: dto.ActivationId,
            ServerId: dto.ServerId,
            TransportProfileId: dto.TransportProfileId,
            TransportProfileName: dto.TransportProfileName,
            Status: dto.Status,
            LocalPriority: dto.LocalPriority,
            RealityPublicKey: dto.RealityPublicKey,
            ActivatedAtUtc: dto.ActivatedAtUtc,
            CreatedAtUtc: dto.CreatedAtUtc,
            UpdatedAtUtc: dto.UpdatedAtUtc,
            Version: dto.Version);

    public static IReadOnlyList<ServerTransportActivationApiResponse> ToApiResponse(
        this IReadOnlyList<ServerTransportActivationListItemDto> items)
        => items.Select(ToApiResponse).ToList();
}
