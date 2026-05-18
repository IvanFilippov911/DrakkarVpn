using DrakkarVpn.Admin.Api.Application.DTOs.ServerTransportActivations;
using DrakkarVpn.Admin.Api.Application.Features.Commands.ServerTransportActivations.ActivateServerTransportActivation;
using DrakkarVpn.Servers.Application.Abstractions.Services;
using DrakkarVpn.Servers.Application.Abstractions.Services.ServerTransportActivations;
using DrakkarVpn.Servers.Application.DTOs.ServerTransportActivations;
using DrakkarVpn.Servers.Domain.Enums.TransportProfile;
using MediatR;

namespace DrakkarVpn.Admin.Api.Application.Features.Commands.ServerTransportActivations.ActivateServerTransport;

public sealed class ActivateServerTransportHandler
    : IRequestHandler<ActivateServerTransportRequest, ActivateServerTransportResultDto>
{
    private readonly IServerTransportActivationService _transportService;
    private readonly IServerTransportApplyJobService _jobService;

    public ActivateServerTransportHandler(
        IServerTransportActivationService transportService,
        IServerTransportApplyJobService jobService)
    {
        _transportService = transportService;
        _jobService = jobService;
    }

    public async Task<ActivateServerTransportResultDto> Handle(
        ActivateServerTransportRequest command,
        CancellationToken ct)
    {
        var targetVersion = await _transportService.ActivateAsync(
            new ActivateServerTransportActivationInput(
                ServerId: command.ServerId,
                ActivationId: command.ActivationId),
            ct);

        var jobId = await _jobService.EnqueueAsync(
            command.ServerId,
            command.ActivationId,
            targetVersion,
            ct);

        var pollUrl =
            $"/api/admin/servers/{command.ServerId}/transport-apply-jobs/{jobId}";

        return new ActivateServerTransportResultDto(
            JobId: jobId,
            ServerId: command.ServerId,
            ActivationId: command.ActivationId,
            TargetTransportVersion: targetVersion,
            Status: ServerTransportApplyJobStatus.Pending,
            PollUrl: pollUrl);
    }
}
