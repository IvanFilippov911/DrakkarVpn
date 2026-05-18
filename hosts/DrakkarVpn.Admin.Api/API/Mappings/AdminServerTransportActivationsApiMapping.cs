using DrakkarVpn.Admin.Api.API.Contracts.ServerTransportActivations;
using DrakkarVpn.Admin.Api.Application.DTOs.ServerTransportActivations;
using DrakkarVpn.Admin.Api.Application.Features.Commands.ServerTransportActivations.ActivateServerTransportActivation;
using DrakkarVpn.Admin.Api.Application.Features.Commands.ServerTransportActivations.AttachServerTransport;
using DrakkarVpn.Admin.Api.Application.Features.Commands.ServerTransportActivations.DeleteServerTransportActivation;
using DrakkarVpn.Admin.Api.Application.Features.Commands.ServerTransportActivations.UpdateServerTransportActivation;
using DrakkarVpn.Servers.Application.DTOs.ServerTransportActivations;

namespace DrakkarVpn.Admin.Api.API.Mappings;

public static class AdminServerTransportActivationsApiMapping
{
    public static AttachServerTransportRequest ToCommand(
        this AttachServerTransportApiRequest request,
        Guid serverId)
        => new(
            ServerId: serverId,
            Profiles: request.Profiles
                .Select(x => new AttachServerTransportRequestItem(
                    x.TransportProfileId,
                    x.RealityPublicKey,
                    x.LocalPriority))
                .ToList(),
            ActivateProfileId: request.ActivateProfileId);

    public static UpdateServerTransportActivationRequest ToCommand(
        this UpdateServerTransportApiRequest request,
        Guid serverId,
        Guid activationId)
        => new(
            ServerId: serverId,
            ActivationId: activationId,
            RealityPublicKey: request.RealityPublicKey,
            LocalPriority: request.LocalPriority);

    public static ServerTransportApiResponse ToApiResponse(this ServerTransportActivationListItemDto dto)
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

    public static IReadOnlyList<ServerTransportApiResponse> ToApiResponse(
        this IReadOnlyList<ServerTransportActivationListItemDto> items)
        => items.Select(ToApiResponse).ToList();

    public static ServerTransportApplyJobAcceptedApiResponse ToApiResponse(
        this ActivateServerTransportResultDto dto)
        => new(
            JobId: dto.JobId,
            ServerId: dto.ServerId,
            ActivationId: dto.ActivationId,
            TargetTransportVersion: dto.TargetTransportVersion,
            Status: dto.Status,
            PollUrl: dto.PollUrl);

    public static ServerTransportApplyJobStatusApiResponse ToApiResponse(
        this ServerTransportApplyJobStatusDto dto)
        => new(
            JobId: dto.JobId,
            ServerId: dto.ServerId,
            ActivationId: dto.ActivationId,
            TargetTransportVersion: dto.TargetTransportVersion,
            State: dto.State,
            LastErrorCode: dto.LastErrorCode,
            LastErrorMessage: dto.LastErrorMessage);
}
