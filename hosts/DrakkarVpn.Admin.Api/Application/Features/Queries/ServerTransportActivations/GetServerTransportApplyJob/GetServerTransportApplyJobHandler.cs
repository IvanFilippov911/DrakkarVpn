using DrakkarVpn.Admin.Api.Application.DTOs.ServerTransportActivations;
using DrakkarVpn.Servers.Application.Abstractions.Services;
using MediatR;

namespace DrakkarVpn.Admin.Api.Application.Features.Queries.ServerTransportActivations.GetServerTransportApplyJob;

public sealed class GetServerTransportApplyJobHandler
    : IRequestHandler<GetServerTransportApplyJobQuery, ServerTransportApplyJobStatusDto?>
{
    private readonly IServerTransportApplyJobService _jobs;

    public GetServerTransportApplyJobHandler(IServerTransportApplyJobService jobs)
        => _jobs = jobs;

    public async Task<ServerTransportApplyJobStatusDto?> Handle(
        GetServerTransportApplyJobQuery query,
        CancellationToken ct)
    {
        var job = await _jobs.GetAsync(query.JobId, ct);
        if (job is null || job.ServerId != query.ServerId)
            return null;

        return new ServerTransportApplyJobStatusDto(
            JobId: job.JobId,
            ServerId: job.ServerId,
            ActivationId: job.ActivationId,
            TargetTransportVersion: job.TargetTransportVersion,
            State: job.State,
            LastErrorCode: job.LastErrorCode,
            LastErrorMessage: job.LastErrorMessage);
    }
}
